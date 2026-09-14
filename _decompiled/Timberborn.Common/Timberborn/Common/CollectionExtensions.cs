using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Timberborn.Common;

public static class CollectionExtensions
{
	public static bool IsEmpty<T>(this T[] collection)
	{
		return collection.Length == 0;
	}

	public static bool IsNullOrEmpty<T>(this T[] collection)
	{
		if (collection != null)
		{
			return collection.Length == 0;
		}
		return true;
	}

	public static void Fill<T>(this T[] collection, Func<T> valueGetter)
	{
		for (int i = 0; i < collection.Length; i++)
		{
			collection[i] = valueGetter();
		}
	}

	public static bool IsEmpty<T>(this IReadOnlyCollection<T> collection)
	{
		return collection.Count == 0;
	}

	public static IEnumerable<T> AsReadOnlyEnumerable<T>(this IEnumerable<T> source)
	{
		return source.Select((T x) => x);
	}

	public static bool AllAreEqual<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
	{
		comparer = comparer ?? EqualityComparer<T>.Default;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		if (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			while (enumerator.MoveNext())
			{
				if (!comparer.Equals(enumerator.Current, current))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static string CollectionToString<T>(this IEnumerable<T> source, string collectionName)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(collectionName + ":");
		foreach (T item in source)
		{
			stringBuilder.AppendLine($"  {item}");
		}
		return stringBuilder.ToString();
	}

	public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
	{
		if (collection is List<T> list)
		{
			list.AddRange(values);
			return;
		}
		foreach (T value in values)
		{
			collection.Add(value);
		}
	}

	public static void AddRange<T>(this ICollection<T> collection, IReadOnlyList<T> values)
	{
		if (collection is List<T> list)
		{
			list.AddRange(values);
			return;
		}
		for (int i = 0; i < values.Count; i++)
		{
			collection.Add(values[i]);
		}
	}

	public static T? MinOrNullable<T>(this IEnumerable<T> source) where T : struct, IComparable<T>
	{
		return source.Select((Func<T, T?>)((T element) => element)).DefaultIfEmpty().Min();
	}

	public static void RemoveLast(this IList list)
	{
		list.RemoveAt(list.Count - 1);
	}

	public static int IndexOf<T>(this IReadOnlyList<T> source, T obj)
	{
		for (int i = 0; i < source.Count; i++)
		{
			if (source[i].Equals(obj))
			{
				return i;
			}
		}
		return -1;
	}

	public static int IndexOf<T>(this ICollection<T> source, T obj)
	{
		int num = 0;
		foreach (T item in source)
		{
			if (item.Equals(obj))
			{
				return num;
			}
			num++;
		}
		return -1;
	}
}
