using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;

namespace Timberborn.Benchmarking;

internal class StatisticsCalculator
{
	public float Median(IEnumerable<float> numbers)
	{
		List<float> list = OrderedList(numbers);
		if (list.IsEmpty())
		{
			throw new ArgumentException("Can't calculate median of an empty sequence");
		}
		if (!HasOddNumberOfElements(list))
		{
			return GetAverageOfTwoMiddleElements(list);
		}
		return GetMiddleElement(list);
	}

	public float Percentile(IEnumerable<float> numbers, float percentile)
	{
		if (percentile < 0f || percentile > 100f)
		{
			throw new ArgumentException("Percentile should be a number between 0 and 100");
		}
		List<float> list = OrderedList(numbers);
		if (list.IsEmpty())
		{
			throw new ArgumentException("Can't calculate percentile of an empty sequence");
		}
		float num = percentile / 100f;
		int num2 = list.Count - 1;
		int index = (int)Math.Round(num * (float)num2);
		return list[index];
	}

	private static List<float> OrderedList(IEnumerable<float> numbers)
	{
		return numbers.OrderBy((float number) => number).ToList();
	}

	private static bool HasOddNumberOfElements<T>(IReadOnlyCollection<T> collection)
	{
		return collection.Count % 2 == 1;
	}

	private static T GetMiddleElement<T>(IReadOnlyList<T> list)
	{
		return list[list.Count / 2];
	}

	private static float GetAverageOfTwoMiddleElements(IReadOnlyList<float> orderedNumbers)
	{
		int num = orderedNumbers.Count / 2;
		float num2 = orderedNumbers[num];
		int index = num - 1;
		return (orderedNumbers[index] + num2) / 2f;
	}
}
