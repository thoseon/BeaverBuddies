using System.Collections.Generic;

namespace Timberborn.Common;

public static class HashSetExtensions
{
	public static void CopyTo<T>(this HashSet<T> source, List<T> target)
	{
		foreach (T item in source)
		{
			target.Add(item);
		}
	}

	public static ReadOnlyHashSet<T> AsReadOnlyHashSet<T>(this HashSet<T> set)
	{
		return new ReadOnlyHashSet<T>(set);
	}
}
