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

	public static bool IsIntersecting(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
	{
		float denominator = ((b.x - a.y) * (d.y - c.y)) - ((b.y - a.y) * (d.x - c.x));
		float numerator1 = ((a.y - c.y) * (d.x - c.x)) - ((a.x - c.x) * (d.y - c.y));
		float numerator2 = ((a.y - c.y) * (b.x - a.x)) - ((a.x - c.x) * (b.y - a.y));

		// Detect coincident lines (has a problem, read below)
		if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

		float r = numerator1 / denominator;
		float s = numerator2 / denominator;

		return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
	}
}
