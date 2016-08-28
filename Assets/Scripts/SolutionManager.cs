using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

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

	public void GenerateNewSolution(int index = 0)
	{
		int straightLimit = Random.Range(2,3);
		int branchOut = Random.Range(0, 3);
		Debug.Log("Generating solution! " + straightLimit + " branch " + branchOut);
		Solutions[index].Clear();

		// try to make big line (up to limit)
		var freeStars = new List<Star>(StarManager.Instance.AllStars);
		Star from = null;
		for (int i = 0; i < straightLimit; i++)
		{
			Star star = GetAvailableStar(Solutions[index], from);
			if (star != null)
			{
				// TODO: check this interesctions check, seems that not working always
				if (from != null && !IntersectsWithExisting(Solutions[index], from, star))
				{
					Solutions[index].Add(new SolutionConnection(from, star));
					freeStars.Remove(from);
				}
				from = star;
			}
		}
		// remove also the last one
		if (from != null)
			freeStars.Remove(from);
		
		// do branching out
		for (int i = 0; i < branchOut; i++)
		{
			BranchOut(Solutions [index], freeStars);
		}
	}

	private void BranchOut(List<SolutionConnection> solution, List<Star> freeStars)
	{
		var branchOutStar = solution.GetRandom().From;
		var freeStar = freeStars.GetRandom();
		solution.Add(new SolutionConnection(branchOutStar, freeStar));
		freeStars.Remove(freeStar);
	}

	private bool IntersectsWithExisting(List<SolutionConnection> solution, Star from, Star to)
	{
		// check all existing connections
		for (int i = 0; i < solution.Count; i++)
		{
			if (Util.IsIntersecting(
				    from.transform.position,
				    to.transform.position,
				    solution [i].From.transform.position,
				    solution [i].To.transform.position
			    ))
			{
				return false;
			}
		}
		return false;
	}

	private Star GetAvailableStar(List<SolutionConnection> solution, Star previousStar)
	{
		// limit it to some value
		int limit = StarManager.Instance.AllStars.Count;
		for(int i = 0; i < limit; i++)
		{
			var star = StarManager.Instance.AllStars.GetRandom();
			// there is no connection already like this
			if (!solution.Exists(s => (s.To == star && s.From == previousStar) || (s.To == previousStar && s.From == star)))
			{
				return star;
			}
		}
		return null;
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
