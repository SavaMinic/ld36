using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class StarManager : MonoBehaviour
{

	public enum GameState
	{
		Default = 0,
		Dragging,
	}

	#region Properties

	public static StarManager Instance { get; private set; }

	public GameState State { get; private set; }

	public List<Star> AllStars { get; private set; }
	public List<Star> SelectedStars { get; private set; }

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
		AllStars = new List<Star>();
		SelectedStars = new List<Star>();
	}

	void Start()
	{
		NewGame();
	}

	void Update()
	{
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
			SelectedStars.Clear();
			ConnectionManager.Instance.DeleteCurrentConnection();
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
		
	public void NewGame(int starCount = 10)
	{
		State = GameState.Default;
		GenerateStars(starCount);
	}

	public void ResetSky()
	{
		for (int i = 0; i < AllStars.Count; i++)
		{
			AllStars[i].Deactivate();
		}
	}

	public void GenerateStars(int count = 5)
	{
		for (int i = 0; i < AllStars.Count; i++)
		{
			GameObject.Destroy(AllStars[i].gameObject);
		}

		AllStars.Clear();
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
				okPosition = !AllStars.Exists(s => Vector2.Distance(s.transform.position, newPosition) < minStarDistance);
			} while (!okPosition);
			star.transform.position = newPosition;

			AllStars.Add(star.GetComponent<Star>());
		}
	}

}
