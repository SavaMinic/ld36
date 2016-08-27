using UnityEngine;
using System.Collections.Generic;

public class StarManager : MonoBehaviour
{

	#region Public fields

	public LayerMask starLayer;

	public float minX, maxX;
	public float minY, maxY;
	public float minStarDistance;

	#endregion

	private List<Star> allStars = new List<Star>();

	private GameObject starPrefab;

	void Awake()
	{
		starPrefab = Resources.Load("Prefabs/Star") as GameObject;
	}

	void Start()
	{
		GenerateStars(10);
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, float.MaxValue, starLayer))
			{
				var star = hit.collider.gameObject.GetComponent<Star>();
				if (star != null)
				{
					Debug.Log("HIT STAR");
				}
			}
		}
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
