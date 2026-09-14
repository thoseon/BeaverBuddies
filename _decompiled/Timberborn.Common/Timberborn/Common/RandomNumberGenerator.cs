using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Common;

public class RandomNumberGenerator : IRandomNumberGenerator
{
	public float Range(float inclusiveMin, float inclusiveMax)
	{
		return UnityEngine.Random.Range(inclusiveMin, inclusiveMax);
	}

	public int Range(int inclusiveMin, int exclusiveMax)
	{
		return UnityEngine.Random.Range(inclusiveMin, exclusiveMax);
	}

	public Vector2 InsideUnitCircle()
	{
		return UnityEngine.Random.insideUnitCircle;
	}

	public T GetListElement<T>(IReadOnlyList<T> list)
	{
		return list[Range(0, list.Count)];
	}

	public bool TryGetListElement<T>(IReadOnlyList<T> list, out T randomElement)
	{
		if (list.Count == 0)
		{
			randomElement = default(T);
			return false;
		}
		randomElement = GetListElement(list);
		return true;
	}

	public T GetEnumerableElement<T>(IEnumerable<T> source)
	{
		(T current, int count) tuple = RandomElement(source);
		var (result, _) = tuple;
		if (tuple.count == 0)
		{
			throw new ArgumentException("Provided enumerable is empty");
		}
		return result;
	}

	public T GetListElementOrDefault<T>(IReadOnlyList<T> list)
	{
		if (!TryGetListElement(list, out var randomElement))
		{
			return default(T);
		}
		return randomElement;
	}

	public bool TryGetEnumerableElement<T>(IEnumerable<T> source, out T randomElement)
	{
		(T current, int count) tuple = RandomElement(source);
		var (val, _) = tuple;
		if (tuple.count == 0)
		{
			randomElement = default(T);
			return false;
		}
		randomElement = val;
		return true;
	}

	public bool CheckProbability(float normalizedProbability)
	{
		if (Mathf.Approximately(normalizedProbability, 1f))
		{
			return true;
		}
		return Range(0f, 1f) < normalizedProbability;
	}

	private (T current, int count) RandomElement<T>(IEnumerable<T> source)
	{
		T item = default(T);
		int num = 0;
		foreach (T item2 in source)
		{
			num++;
			if (Range(0, num) == 0)
			{
				item = item2;
			}
		}
		return (current: item, count: num);
	}
}
