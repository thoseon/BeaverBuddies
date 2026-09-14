using System.Collections.Generic;

namespace Timberborn.Common;

public class CyclicBuffer<T>
{
	private readonly Queue<T> _queue;

	private readonly int _size;

	public IEnumerable<T> Values => _queue.AsReadOnlyEnumerable();

	public CyclicBuffer(int size)
	{
		_queue = new Queue<T>(size);
		_size = size;
	}

	public void Add(T value)
	{
		if (_queue.Count == _size)
		{
			_queue.Dequeue();
		}
		_queue.Enqueue(value);
	}

	public void AddRange(IEnumerable<T> values)
	{
		foreach (T value in values)
		{
			Add(value);
		}
	}
}
