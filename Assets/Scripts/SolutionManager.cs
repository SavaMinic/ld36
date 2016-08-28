using UnityEngine;
using System.Collections.Generic;
using System;

public class SolutionManager : MonoBehaviour
{

	public class Solution
	{
		public Star From;
		public Star To;

		public Solution(Star from, Star to)
		{
			From = from;
			To = to;
		}
	}

	#region Properties

	public static SolutionManager Instance { get; private set; }

	public List<Solution> RequiredConnections { get; private set; }

	#endregion

	void Awake()
	{
		Instance = this;
		RequiredConnections = new List<Solution>();
	}

	#region Public methods

	public void GenerateNewSolution(int connectionsCount = 5)
	{
		Debug.Log("Generating solution! " + connectionsCount);

		RequiredConnections.Clear();

		var starWeights = new Dictionary<Star, int>();
		StarManager.Instance.AllStars.ForEach(s => starWeights.Add(s, 3));

		Star from = null;
		for (int i = 0; i < connectionsCount; i++)
		{
			var star = StarManager.Instance.AllStars.PickRandomFromWeightedList(s => starWeights[s]);
			starWeights[star] -= 1;
			if (from != null)
			{
				RequiredConnections.Add(new Solution(from, star));
			}
			from = star;
		}
	}

	// just for test
	public void ShowSolution()
	{
		ConnectionManager.Instance.DeleteAllConnections();
		for (int i = 0; i < RequiredConnections.Count; i++)
		{
			ConnectionManager.Instance.CreateConnection(RequiredConnections[i].From, RequiredConnections[i].To);
		}
	}

	#endregion

}
