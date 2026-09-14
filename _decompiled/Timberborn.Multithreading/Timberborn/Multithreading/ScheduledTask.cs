using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Timberborn.Multithreading;

internal class ScheduledTask<T> : IScheduledTask where T : struct, ITaskRunner
{
	private readonly ScheduledTaskPool _scheduledTaskPool;

	private readonly ISnapshotCollector _snapshotCollector;

	private int _version;

	private T _task;

	private int _nextRunIndex;

	private int _completedRuns;

	private int _prerequisites;

	private readonly List<IScheduledTask> _dependents = new List<IScheduledTask>();

	private readonly object _lockObject = new object();

	public ScheduledTask(ScheduledTaskPool scheduledTaskPool, ISnapshotCollector snapshotCollector)
	{
		_scheduledTaskPool = scheduledTaskPool;
		_snapshotCollector = snapshotCollector;
	}

	public ParallelizerHandle Initialize(Parallelizer parallelizer, in T task, ReadOnlySpan<ParallelizerHandle> dependencies)
	{
		parallelizer.IncrementPendingTasks();
		lock (_lockObject)
		{
			_task = task;
			ReadOnlySpan<ParallelizerHandle> readOnlySpan = dependencies;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ParallelizerHandle parallelizerHandle = readOnlySpan[i];
				if (parallelizerHandle.Task.AddDependent(parallelizerHandle.Version, this))
				{
					_prerequisites++;
				}
			}
			if (_prerequisites == 0)
			{
				AddToReadyTasks(parallelizer);
			}
			return new ParallelizerHandle(this, Volatile.Read(ref _version), parallelizer);
		}
	}

	public bool AddDependent(int expectedVersion, IScheduledTask dependent)
	{
		lock (_lockObject)
		{
			if (_version == expectedVersion)
			{
				_dependents.Add(dependent);
				return true;
			}
		}
		return false;
	}

	public void Run(Parallelizer parallelizer)
	{
		int runIndex;
		lock (_lockObject)
		{
			runIndex = _nextRunIndex++;
		}
		RunTask(runIndex);
		lock (_lockObject)
		{
			_completedRuns++;
			if (_completedRuns == _task.ExpectedRuns)
			{
				AdvanceDependents(parallelizer);
				Interlocked.Increment(ref _version);
				_scheduledTaskPool.Return(this);
				parallelizer.DecrementPendingTasks();
			}
		}
	}

	public void AdvancePrerequisites(Parallelizer parallelizer)
	{
		lock (_lockObject)
		{
			_prerequisites--;
			if (_prerequisites == 0)
			{
				AddToReadyTasks(parallelizer);
			}
		}
	}

	public void Reset()
	{
		lock (_lockObject)
		{
			_task = default(T);
			_nextRunIndex = 0;
			_completedRuns = 0;
			_prerequisites = 0;
			_dependents.Clear();
		}
	}

	private void RunTask(int runIndex)
	{
		if (_snapshotCollector.IsCollecting)
		{
			long timestamp = Stopwatch.GetTimestamp();
			_task.Run(runIndex);
			long timestamp2 = Stopwatch.GetTimestamp();
			_snapshotCollector.AddTaskSample(runIndex, _task.ExpectedRuns, timestamp, timestamp2, _task.GetType());
		}
		else
		{
			_task.Run(runIndex);
		}
	}

	private void AdvanceDependents(Parallelizer parallelizer)
	{
		foreach (IScheduledTask dependent in _dependents)
		{
			dependent.AdvancePrerequisites(parallelizer);
		}
		_dependents.Clear();
	}

	private void AddToReadyTasks(Parallelizer parallelizer)
	{
		int expectedRuns = _task.ExpectedRuns;
		for (int i = 0; i < expectedRuns; i++)
		{
			parallelizer.AddReadyTask(this);
		}
	}
}
