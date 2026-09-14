using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Timberborn.Common;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Multithreading;

internal class Parallelizer : IParallelizer, ILoadableSingleton, IUnloadableSingleton
{
	private readonly ScheduledTaskPool _scheduledTaskPool;

	private readonly LockingQueue<IScheduledTask> _readyTasks = new LockingQueue<IScheduledTask>();

	private readonly ConcurrentQueue<ParallelizerExceptionLog> _exceptions = new ConcurrentQueue<ParallelizerExceptionLog>();

	private readonly ParallelizerHandle[] _reusableOneDepdendencyArray = new ParallelizerHandle[1];

	private int _pendingTasks;

	private long _lastTaskTimestamp;

	private ImmutableArray<Thread> _threads;

	private Thread _mainThread;

	private bool _closed;

	private bool _scheduling;

	public int NumberOfThreads { get; private set; }

	public long LastTaskTimestamp => _lastTaskTimestamp;

	public Parallelizer(ScheduledTaskPool scheduledTaskPool)
	{
		_scheduledTaskPool = scheduledTaskPool;
	}

	public void Load()
	{
		_mainThread = Thread.CurrentThread;
		NumberOfThreads = CalculateNumberOfThreads();
		if (!Application.isEditor)
		{
			UnityEngine.Debug.Log($"Number of threads: {NumberOfThreads}");
		}
		List<Thread> list = new List<Thread>();
		for (int i = 0; i < NumberOfThreads; i++)
		{
			Thread item = new Thread(ThreadStart)
			{
				Name = string.Format("{0}-{1}", "Parallelizer", i)
			};
			list.Add(item);
		}
		_threads = list.ToImmutableArray();
		foreach (Thread thread in _threads)
		{
			thread.Start();
		}
	}

	public void Unload()
	{
		CloseThreads();
		ThrowIfHasExceptions();
	}

	public ParallelizerHandle Schedule<T>(in T task) where T : struct, IParallelizerSingleTask
	{
		return Schedule(in task, ReadOnlySpan<ParallelizerHandle>.Empty);
	}

	public ParallelizerHandle Schedule<T>(in T task, ParallelizerHandle dependency) where T : struct, IParallelizerSingleTask
	{
		_reusableOneDepdendencyArray[0] = dependency;
		try
		{
			return Schedule(in task, MemoryExtensions.AsSpan(_reusableOneDepdendencyArray));
		}
		finally
		{
			_reusableOneDepdendencyArray[0] = default(ParallelizerHandle);
		}
	}

	public ParallelizerHandle Schedule<T>(in T task, ReadOnlySpan<ParallelizerHandle> dependencies) where T : struct, IParallelizerSingleTask
	{
		ValidateScheduling();
		return _scheduledTaskPool.Rent<SingleTaskRunner<T>>().Initialize(this, new SingleTaskRunner<T>(task), dependencies);
	}

	public ParallelizerHandle Schedule<T>(int fromInclusive, int toExclusive, int batchSize, in T task) where T : struct, IParallelizerLoopTask
	{
		return Schedule(fromInclusive, toExclusive, batchSize, in task, ReadOnlySpan<ParallelizerHandle>.Empty);
	}

	public ParallelizerHandle Schedule<T>(int fromInclusive, int toExclusive, int batchSize, in T task, ParallelizerHandle dependency) where T : struct, IParallelizerLoopTask
	{
		_reusableOneDepdendencyArray[0] = dependency;
		try
		{
			return Schedule(fromInclusive, toExclusive, batchSize, in task, MemoryExtensions.AsSpan(_reusableOneDepdendencyArray));
		}
		finally
		{
			_reusableOneDepdendencyArray[0] = default(ParallelizerHandle);
		}
	}

	public ParallelizerHandle Schedule<T>(int fromInclusive, int toExclusive, int batchSize, in T task, ReadOnlySpan<ParallelizerHandle> dependencies) where T : struct, IParallelizerLoopTask
	{
		ValidateScheduling();
		ValidateLoopRange<T>(fromInclusive, toExclusive);
		ValidateBatchSize<T>(batchSize);
		return _scheduledTaskPool.Rent<LoopTaskRunner<T>>().Initialize(this, new LoopTaskRunner<T>(task, fromInclusive, toExclusive, batchSize), dependencies);
	}

	public void StartScheduling()
	{
		if (_scheduling)
		{
			throw new InvalidOperationException("Already in scheduling phase.");
		}
		_scheduling = true;
		ThrowIfAnyPendingTasks();
	}

	public void StopScheduling()
	{
		_scheduling = false;
	}

	public void Wait()
	{
		ThrowIfNotMainThread();
		SpinWait spinWait = default(SpinWait);
		while (Interlocked.CompareExchange(ref _pendingTasks, 0, 0) > 0)
		{
			ThrowIfHasExceptions();
			spinWait.SpinOnce();
		}
		ThrowIfHasExceptions();
		ThrowIfAnyPendingTasks();
	}

	public void ThrowIfAnyPendingTasks()
	{
		int count = _readyTasks.Count;
		if (Interlocked.CompareExchange(ref _pendingTasks, 0, 0) > 0 || count > 0)
		{
			throw new ParallelizerException($"Unexpected pending tasks! Counter: {_pendingTasks}. Queue: {count}");
		}
	}

	internal void AddReadyTask(IScheduledTask scheduledTask)
	{
		_readyTasks.Add(scheduledTask);
	}

	internal void IncrementPendingTasks()
	{
		Interlocked.Increment(ref _pendingTasks);
	}

	internal void DecrementPendingTasks()
	{
		Interlocked.Decrement(ref _pendingTasks);
	}

	private void ThreadStart()
	{
		try
		{
			IScheduledTask item;
			while (_readyTasks.TryTakeBlocking(out item) && _exceptions.IsEmpty)
			{
				ExecuteTask(item);
			}
		}
		catch (Exception exception)
		{
			EnqueueException(exception);
		}
	}

	private void ExecuteTask(IScheduledTask scheduledTask)
	{
		scheduledTask.Run(this);
		Interlocked.Exchange(ref _lastTaskTimestamp, Stopwatch.GetTimestamp());
	}

	private void CloseThreads()
	{
		if (_closed)
		{
			UnityEngine.Debug.Log("Threads already closed");
			return;
		}
		_readyTasks.CompleteAdding();
		if (_threads != null)
		{
			foreach (Thread thread in _threads)
			{
				thread.Join();
			}
		}
		_closed = true;
	}

	private void ThrowIfHasExceptions()
	{
		ThrowIfNotMainThread();
		if (!_exceptions.IsEmpty)
		{
			UnityEngine.Debug.Log("Closing threads due to uncaught exceptions");
			CloseThreads();
			UnityEngine.Debug.Log("Threads closed");
			throw ParallelizerException.From(CollectExceptions());
		}
	}

	private void ThrowIfNotMainThread()
	{
		if (Thread.CurrentThread != _mainThread)
		{
			throw new InvalidOperationException("This operation can only be called on the main thread.");
		}
	}

	private ImmutableArray<ParallelizerExceptionLog> CollectExceptions()
	{
		List<ParallelizerExceptionLog> list = new List<ParallelizerExceptionLog>();
		ParallelizerExceptionLog result;
		while (_exceptions.TryDequeue(out result))
		{
			list.Add(result);
		}
		return list.ToImmutableArray();
	}

	private void EnqueueException(Exception exception)
	{
		_exceptions.Enqueue(new ParallelizerExceptionLog(exception, Thread.CurrentThread.DisplayName()));
	}

	private void ValidateScheduling()
	{
		if (!_scheduling)
		{
			throw new InvalidOperationException("Cannot schedule tasks outside of schedule phase.");
		}
	}

	private static void ValidateLoopRange<T>(int fromInclusive, int toExclusive)
	{
		if (toExclusive <= fromInclusive)
		{
			throw new ArgumentException(string.Format("{0} {1}", "toExclusive", toExclusive) + string.Format(" must be greater than {0} {1}", "fromInclusive", fromInclusive) + " when scheduling T");
		}
	}

	private static void ValidateBatchSize<T>(int batchSize)
	{
		if (batchSize < 1)
		{
			throw new ArgumentException(string.Format("{0} {1} must be at least 1", "batchSize", batchSize) + " when scheduling T");
		}
	}

	private static int CalculateNumberOfThreads()
	{
		return Math.Clamp(ProcessorInfo.GetPhysicalProcessorCount() - 1, 3, 8);
	}

	ParallelizerHandle IParallelizer.Schedule<T>(in T task)
	{
		return Schedule(in task);
	}

	ParallelizerHandle IParallelizer.Schedule<T>(in T task, ParallelizerHandle dependency)
	{
		return Schedule(in task, dependency);
	}

	ParallelizerHandle IParallelizer.Schedule<T>(in T task, ReadOnlySpan<ParallelizerHandle> dependencies)
	{
		return Schedule(in task, dependencies);
	}

	ParallelizerHandle IParallelizer.Schedule<T>(int fromInclusive, int toExclusive, int batchSize, in T task)
	{
		return Schedule(fromInclusive, toExclusive, batchSize, in task);
	}

	ParallelizerHandle IParallelizer.Schedule<T>(int fromInclusive, int toExclusive, int batchSize, in T task, ParallelizerHandle dependency)
	{
		return Schedule(fromInclusive, toExclusive, batchSize, in task, dependency);
	}

	ParallelizerHandle IParallelizer.Schedule<T>(int fromInclusive, int toExclusive, int batchSize, in T task, ReadOnlySpan<ParallelizerHandle> dependencies)
	{
		return Schedule(fromInclusive, toExclusive, batchSize, in task, dependencies);
	}
}
