using System.Collections.Generic;
using System.Threading;

namespace Timberborn.Multithreading;

internal class LockingQueue<T>
{
	private readonly object _lockObject = new object();

	private readonly Queue<T> _queue = new Queue<T>();

	private bool _addingCompleted;

	public int Count
	{
		get
		{
			lock (_lockObject)
			{
				return _queue.Count;
			}
		}
	}

	public void Add(T item)
	{
		lock (_lockObject)
		{
			_queue.Enqueue(item);
			Monitor.Pulse(_lockObject);
		}
	}

	public void CompleteAdding()
	{
		lock (_lockObject)
		{
			_addingCompleted = true;
			Monitor.PulseAll(_lockObject);
		}
	}

	public bool TryTakeBlocking(out T item)
	{
		lock (_lockObject)
		{
			while (!_queue.TryDequeue(out item))
			{
				if (_addingCompleted)
				{
					item = default(T);
					return false;
				}
				Monitor.Wait(_lockObject);
			}
			return true;
		}
	}
}
