using System.Collections;
using System.Collections.Generic;

namespace Timberborn.Common;

public readonly struct ReadOnlyList<T> : IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
{
	private readonly List<T> _list;

	public int Count => _list.Count;

	public T this[int index] => _list[index];

	internal ReadOnlyList(List<T> list)
	{
		_list = list;
	}

	public List<T>.Enumerator GetEnumerator()
	{
		return _list.GetEnumerator();
	}

	public bool Contains(T item)
	{
		return _list.Contains(item);
	}

	public bool IsEmpty()
	{
		return _list.Count == 0;
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
