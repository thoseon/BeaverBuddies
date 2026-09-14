using System;
using System.Collections.Generic;

namespace Timberborn.Common;

public static class FastCollectionExtensions
{
	public static int FastCount<T>(this IReadOnlyList<T> source, Predicate<T> predicate)
	{
		int num = 0;
		for (int i = 0; i < source.Count; i++)
		{
			if (predicate(source[i]))
			{
				num++;
			}
		}
		return num;
	}

	public static bool FastAll<T>(this IReadOnlyList<T> source, Predicate<T> predicate)
	{
		for (int i = 0; i < source.Count; i++)
		{
			if (!predicate(source[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool FastAny<T>(this IReadOnlyList<T> source, Predicate<T> predicate)
	{
		for (int i = 0; i < source.Count; i++)
		{
			if (predicate(source[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool FastContains<T>(this IReadOnlyList<T> source, T element) where T : IEquatable<T>
	{
		for (int i = 0; i < source.Count; i++)
		{
			if (source[i].Equals(element))
			{
				return true;
			}
		}
		return false;
	}
}
