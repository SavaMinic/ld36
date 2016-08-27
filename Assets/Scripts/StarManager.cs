using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class StarManager : MonoBehaviour
{

	#region Properties

	public static StarManager Instance { get; private set; }

	public enum GameState
	{
		Default = 0,
		Dragging,
	}

	public GameState State { get; private set; }

	#endregion

	#region Public fields

	public LayerMask starLayer;

	public float minX, maxX;
	public float minY, maxY;
	public float minStarDistance;

	#endregion

	#region Private fields

	private List<Star> allStars = new List<Star>();

	private List<Star> selectedStars = new List<Star>();
	private Star lastSelectedStar;

	private GameObject starPrefab;

	#endregion

	void Awake()
	{
		Instance = this;
		starPrefab = Resources.Load("Prefabs/Star") as GameObject;
	}

	void Start()
	{
		State = GameState.Default;
		GenerateStars(10);
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0) && State == GameState.Default)
		{
			OnStarHit(star =>
			{
				Debug.Log("HIT STAR " + star.gameObject.transform.position);
				State = GameState.Dragging;
				lastSelectedStar = star;
				star.Select();
				selectedStars.Add(star);
			});
		}
		else if (Input.GetMouseButtonUp(0) && State == GameState.Dragging)
		{
			Debug.Log("Released!");
			State = GameState.Default;
			for (int i = 0; i < selectedStars.Count; i++)
			{
				selectedStars[i].Deselect();
			}
			selectedStars.Clear();
		}
		else if (Input.GetMouseButton(0) && State == GameState.Dragging)
		{
			OnStarHit(star =>
			{
				if (star != lastSelectedStar || lastSelectedStar == null)
				{
					Debug.Log("Toggle STAR " + star.gameObject.transform.position);
					lastSelectedStar = star;
					if (!star.IsSelected)
					{
						star.Select();
						selectedStars.Add(star);
					}
					else
					{
						star.Deselect();
						selectedStars.Remove(star);
					}
				}
			}, () => {
				lastSelectedStar = null;
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
		else nothingCallback();
	}

	public void GenerateStars(int count = 5)
	{
		for (int i = 0; i < allStars.Count; i++)
		{
			GameObject.Destroy(allStars[i].gameObject);
		}

		allStars.Clear();
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
				okPosition = !allStars.Exists(s => Vector2.Distance(s.transform.position, newPosition) < minStarDistance);
			} while (!okPosition);
			star.transform.position = newPosition;

			allStars.Add(star.GetComponent<Star>());
		}
	}

}
