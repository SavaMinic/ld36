using UnityEngine;
using System.Collections.Generic;
using System;

public static class Util
{

	public static T PickRandomFromWeightedList<T>(this IEnumerable<T> enumerable, Func<T, int> weightFunc)
	{
		var random = new System.Random(Guid.NewGuid().GetHashCode());
		int totalWeight = 0; // this stores sum of weights of all elements before current
		T selected = default(T); // currently selected element
		foreach (var data in enumerable)
		{
			int weight = weightFunc(data);
			int r = random.Next(totalWeight + weight);
			// probability of this is weight/(totalWeight+weight)
			// it is the probability of discarding last selected element and selecting current one instead
			if (r >= totalWeight)
				selected = data;
			totalWeight += weight;
		}

		return selected; 
	}

	public static T GetRandom<T>(this List<T> list)
	{
		if (list.Count == 0) return default(T);
		int index = UnityEngine.Random.Range(0, list.Count);
		return list[index];
	}
}
