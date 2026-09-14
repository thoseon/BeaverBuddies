using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.ObjectPool;

namespace Timberborn.Multithreading;

internal class ScheduledTaskPool
{
	private class ScheduledTaskPooledObjectPolicy<T> : IPooledObjectPolicy<ScheduledTask<T>> where T : struct, ITaskRunner
	{
		private readonly ScheduledTaskPool _scheduledTaskPool;

		private readonly ISnapshotCollector _snapshotCollector;

		public ScheduledTaskPooledObjectPolicy(ScheduledTaskPool scheduledTaskPool, ISnapshotCollector snapshotCollector)
		{
			_scheduledTaskPool = scheduledTaskPool;
			_snapshotCollector = snapshotCollector;
		}

		public ScheduledTask<T> Create()
		{
			return new ScheduledTask<T>(_scheduledTaskPool, _snapshotCollector);
		}

		public bool Return(ScheduledTask<T> task)
		{
			task.Reset();
			return true;
		}
	}

	private readonly ISnapshotCollector _snapshotCollector;

	private readonly ConcurrentDictionary<Type, object> _pools = new ConcurrentDictionary<Type, object>();

	public ScheduledTaskPool(ISnapshotCollector snapshotCollector)
	{
		_snapshotCollector = snapshotCollector;
	}

	public ScheduledTask<T> Rent<T>() where T : struct, ITaskRunner
	{
		return GetOrAddPool<T>().Get();
	}

	public void Return<T>(ScheduledTask<T> task) where T : struct, ITaskRunner
	{
		GetOrAddPool<T>().Return(task);
	}

	private DefaultObjectPool<ScheduledTask<T>> GetOrAddPool<T>() where T : struct, ITaskRunner
	{
		Type typeFromHandle = typeof(T);
		if (!_pools.TryGetValue(typeFromHandle, out var value))
		{
			Func<Type, ScheduledTaskPool, object> valueFactory = CreatePool<T>;
			value = (DefaultObjectPool<ScheduledTask<T>>)_pools.GetOrAdd(typeFromHandle, valueFactory, this);
		}
		return (DefaultObjectPool<ScheduledTask<T>>)value;
	}

	private object CreatePool<T>(Type type, ScheduledTaskPool scheduledTaskPool) where T : struct, ITaskRunner
	{
		return CreatePool<T>(scheduledTaskPool, _snapshotCollector);
	}

	private static DefaultObjectPool<ScheduledTask<T>> CreatePool<T>(ScheduledTaskPool scheduledTaskPool, ISnapshotCollector snapshotCollector) where T : struct, ITaskRunner
	{
		return new DefaultObjectPool<ScheduledTask<T>>(new ScheduledTaskPooledObjectPolicy<T>(scheduledTaskPool, snapshotCollector), int.MaxValue);
	}
}
