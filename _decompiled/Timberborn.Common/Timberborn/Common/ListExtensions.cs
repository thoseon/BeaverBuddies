using System.Collections.Generic;

namespace Timberborn.Common;

public static class ListExtensions
{
	public static ReadOnlyList<T> AsReadOnlyList<T>(this List<T> list)
	{
		return new ReadOnlyList<T>(list);
	}

	public static void InsertSorted<T>(this List<T> list, T item, IComparer<T> comparer, out int index)
	{
		index = list.BinarySearch(item, comparer);
		if (index < 0)
		{
			index = ~index;
		}
		list.Insert(index, item);
	}
}
