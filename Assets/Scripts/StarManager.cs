using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class StarManager : MonoBehaviour
{

	public enum GameState
	{
		Default = 0,
		Talking,
		Dragging,
		Victory,
		Death
	}

	#region Properties

	public static StarManager Instance { get; private set; }

	public GameState State { get; private set; }
	public bool IsPlaying { get { return State == GameState.Default || State == GameState.Dragging; } }

	public List<List<Star>> AllStars { get; private set; }
	public List<Star> SelectedStars { get; private set; }

	public int WinningIndex { get; private set; }

	public string CurrentProblem { get; private set; }
	public List<string> CurrentAnswers { get; private set; }

	#endregion

	#region Public fields

	public LayerMask starLayer;

	public float minX, maxX;
	public float minY, maxY;
	public float minStarDistance;

	#endregion

	#region Private fields

	private Star lastHoveredStar;

	private GameObject starPrefab;

	#endregion

	void Awake()
	{
		Instance = this;
		starPrefab = Resources.Load<GameObject>("Prefabs/Star");
		AllStars = new List<List<Star>>();
		for (int i = 0; i < 3; i++)
			AllStars.Add(new List<Star>());
		SelectedStars = new List<Star>();
		CurrentAnswers = new List<string>();
	}

	void Start()
	{
		NewGame();
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
			
		// On click, we start to "drag"
		if (Input.GetMouseButtonDown(0) && State == GameState.Default)
		{
			OnStarHit(star =>
			{
				State = GameState.Dragging;
				lastHoveredStar = star;
				star.Activate();
				SelectedStars.Add(star);
				ConnectionManager.Instance.CreateCurrentConnection(star);
			});
		}
		// Released the button while dragging
		else if (Input.GetMouseButtonUp(0) && State == GameState.Dragging)
		{
			State = GameState.Default;
			if (SelectedStars.Count == 1 
				&& !ConnectionManager.Instance.Connections.Exists(c => c.from == SelectedStars[0] || c.to == SelectedStars[0]))
			{
				SelectedStars[0].Deactivate();
			}
			SelectedStars.Clear();
			ConnectionManager.Instance.DeleteCurrentConnection();
			if (CheckVictory())
			{
				Victory();
			}
		}
		// Still dragging
		else if (Input.GetMouseButton(0) && State == GameState.Dragging)
		{
			OnStarHit(star =>
			{
				if (star != lastHoveredStar)
				{
					lastHoveredStar = star;
					if (SelectedStars.Count > 0)
					{
						ConnectionManager.Instance.ConnectCurrentConnection(star);
					}
					// if star is not selected, select it
					if (!star.IsActive)
					{
						star.Activate();
						SelectedStars.Add(star);
					}
				}
			}, () => {
				lastHoveredStar = null;
				ConnectionManager.Instance.UpdateCurrentConnectionPosition();
			});
		}
	}

	private void OnStarHit(Action<Star> callback, Action nothingCallback = null)
	{
		RaycastHit hit;
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, float.MaxValue, starLayer))
		{
			var star = hit.collider.gameObject.GetComponent<Star>();
			if (star != null)
			{
				callback(star);
			}
		}
		else if (nothingCallback != null)
			nothingCallback();
	}
		
	public void NewGame()
	{
		State = GameState.Talking;
		KingManager.Instance.StartTalk(new List<string>() {
			"Hey, you!\n\nYup, you there...",
			"You look smart, with that pointy hat and those glasses.",
			"My previous prophet had an <i>'accident'</i>,\nso we need someone to fill in...",
			"Your job is to answer to all of urgent and kingdom critical questions I have...",
			"... by looking up answers in the sky.",
			"Use scrolls to help you solve the answer.\n\n<b>Drag</b>  to connect stars!",
			//"Don't try to be lazy!\n\nI really dislike lazy people",
			//"They tend to have 'accidents'..."
		}, true);
		GenerateStars();
	}

	public void NewRound(bool generateStars = true)
	{
		State = GameState.Talking;
		if (generateStars)
		{
			GenerateStars();
		}
		ConnectionManager.Instance.DeleteAllConnections();

		// get the problem
		var problem = KingManager.Instance.GetRandomProblem();
		CurrentProblem = problem.Text;
		KingManager.Instance.StartTalk(new List<string>() {
			problem.Text
		});
		// generate answers
		problem.Answers.Shuffle();
		CurrentAnswers.Clear();
		for (int i = 0; i < 3; i++)
		{
			CurrentAnswers.Add(problem.Answers[i]);
		}
	}

	public void StartGame()
	{
		State = GameState.Default;
	}

	public void GenerateStars()
	{
		int starCount = 10;
		WinningIndex = Random.Range(0, 2);
		Debug.Log("Answer is " + WinningIndex);
		GenerateStars(0, starCount);
		SolutionManager.Instance.GenerateNewSolution(0);
		GenerateStars(1, starCount);
		SolutionManager.Instance.GenerateNewSolution(1);
		GenerateStars(2, starCount);
		SolutionManager.Instance.GenerateNewSolution(2);
	}

	public void ResetSky()
	{
		for (int i = 0; i < AllStars[WinningIndex].Count; i++)
		{
			AllStars[WinningIndex][i].Deactivate();
		}
	}

	public void GenerateStars(int index, int count = 5)
	{
		for (int i = 0; i < AllStars[index].Count; i++)
		{
			GameObject.Destroy(AllStars[index][i].gameObject);
		}
		AllStars[index].Clear();

		for (int i = 0; i < count; i++)
		{
			// instantiate star
			var star = Instantiate<GameObject>(starPrefab);
			star.transform.SetParent(transform);

			// find position to it (so it's not too close to other stars)
			var okPosition = false;
			Vector2 newPosition = Vector2.zero;
			do
			{
				newPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
				okPosition = !AllStars[index].Exists(s => Vector2.Distance(s.transform.position, newPosition) < minStarDistance);
			} while (!okPosition);
			star.transform.position = newPosition;
			star.gameObject.SetActive(index == WinningIndex);

			AllStars[index].Add(star.GetComponent<Star>());

		}
	}

	private bool CheckVictory()
	{
		var solution = SolutionManager.Instance.Solutions[WinningIndex];
		var selectedConnections = new List<Connection>(ConnectionManager.Instance.Connections);
		for (int i = 0; i < solution.Count; i++)
		{
			// check if this connection from solution exits in selected ones
			var match = selectedConnections.Find(c => 
				(c.from == solution[i].From && c.to == solution[i].To)
		         || (c.from == solution[i].To && c.to == solution[i].From)
			);
			if (match == null)
				return false;
			selectedConnections.Remove(match);
		}
		// all connection in solution are selected
		// check if there are no extra ones
		return selectedConnections.Count == 0;
	}

	private void Victory()
	{
		State = GameState.Victory;
		KingManager.Instance.StartTalk(new List<string>() {
			"What? Really?\n\n<b><i>" + CurrentAnswers[WinningIndex] + "?</i></b>\n\nInteresting!",
			"I have something more to ask..."
		}, true, true);
	}
}
