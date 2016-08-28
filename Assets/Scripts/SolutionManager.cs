using UnityEngine;
using System.Collections.Generic;
using System;

public class SolutionManager : MonoBehaviour
{

	public class SolutionConnection
	{
		public Star From;
		public Star To;

		public SolutionConnection(Star from, Star to)
		{
			From = from;
			To = to;
		}
	}

	#region Properties

	public static SolutionManager Instance { get; private set; }

	public List<List<SolutionConnection>> Solutions { get; private set; }

	#endregion

	void Awake()
	{
		Instance = this;
		Solutions = new List<List<SolutionConnection>>();
		for (int i = 0; i < 3; i++)
			Solutions.Add(new List<SolutionConnection>());
	}

	#region Public methods

	public void GenerateNewSolution(int index = 0, int connectionsCount = 5)
	{
		Debug.Log("Generating solution! " + connectionsCount);
		Solutions[index].Clear();

		var starWeights = new Dictionary<Star, int>();
		StarManager.Instance.AllStars.ForEach(s => starWeights.Add(s, 3));

		Star from = null;
		for (int i = 0; i < connectionsCount; i++)
		{
			var star = StarManager.Instance.AllStars.PickRandomFromWeightedList(s => starWeights[s]);
			starWeights[star] -= 1;
			if (from != null)
			{
				Solutions[index].Add(new SolutionConnection(from, star));
			}
			from = star;
		}
	}

	// just for test
	public void ShowSolution()
	{
		ConnectionManager.Instance.DeleteAllConnections();
		for (int i = 0; i < Solutions[0].Count; i++)
		{
			ConnectionManager.Instance.CreateConnection(Solutions[0][i].From, Solutions[0][i].To);
		}
	}

	#endregion

}
