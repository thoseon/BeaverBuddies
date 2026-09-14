using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Timberborn.Common;
using Timberborn.Multithreading;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;

namespace Timberborn.MultithreadingAnalysis;

public class SnapshotCollector : ISnapshotCollector, ITickableSingleton
{
	private readonly SpeedManager _speedManager;

	private readonly ConcurrentQueue<TaskSample> _taskSamples = new ConcurrentQueue<TaskSample>();

	private readonly ConcurrentQueue<Marker> _markers = new ConcurrentQueue<Marker>();

	private volatile bool _isCollecting;

	private bool _collectionScheduled;

	private int _remainingTicks;

	private int _scheduledTicks;

	public bool IsCollecting => _isCollecting;

	public event EventHandler<Snapshot> SnapshotCollected;

	public SnapshotCollector(SpeedManager speedManager)
	{
		_speedManager = speedManager;
	}

	public void Tick()
	{
		if (IsCollecting)
		{
			_remainingTicks--;
			if (_remainingTicks <= 0)
			{
				FinishCollection();
				_isCollecting = false;
			}
		}
		else if (_collectionScheduled)
		{
			_isCollecting = true;
			_collectionScheduled = false;
		}
	}

	public void ScheduleCollection(int ticks)
	{
		if (!_collectionScheduled && !IsCollecting)
		{
			_collectionScheduled = true;
			_remainingTicks = (_scheduledTicks = ticks);
			_speedManager.ChangeAndLockSpeed(1f);
		}
	}

	public void AddTaskSample(int run, int totalRuns, long startTimestamp, long endTimestamp, Type type)
	{
		Thread currentThread = Thread.CurrentThread;
		TaskSample item = new TaskSample(run, totalRuns, startTimestamp, endTimestamp, currentThread, type);
		_taskSamples.Enqueue(item);
	}

	public void AddMarker(string id)
	{
		if (IsCollecting)
		{
			_markers.Enqueue(new Marker(id, Stopwatch.GetTimestamp(), Thread.CurrentThread));
		}
	}

	private void FinishCollection()
	{
		SnapshotCollected?.Invoke(this, new Snapshot(_scheduledTicks, _taskSamples.ToList().AsReadOnlyList(), _markers.ToList().AsReadOnlyList()));
		_taskSamples.Clear();
		_markers.Clear();
		_speedManager.UnlockSpeed();
	}
}
