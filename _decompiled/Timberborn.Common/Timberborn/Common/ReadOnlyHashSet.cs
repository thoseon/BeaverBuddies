using System.Collections.Generic;

namespace Timberborn.Common;

public readonly struct ReadOnlyHashSet<T>
{
	private readonly HashSet<T> _set;

	public int Count => _set.Count;

	internal ReadOnlyHashSet(HashSet<T> set)
	{
		_set = set;
	}

	public HashSet<T>.Enumerator GetEnumerator()
	{
		return _set.GetEnumerator();
	}

	public bool Contains(T item)
	{
		return _set.Contains(item);
	}
}
