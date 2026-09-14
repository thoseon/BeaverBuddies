using System;
using System.Collections.Generic;
using Timberborn.TickSystem;

namespace Timberborn.TimeSystem;

internal class TimeTriggerService : ITickableSingleton
{
	private readonly struct SortableKey : IComparable<SortableKey>
	{
		private readonly long _id;

		public float Timestamp { get; }

		public SortableKey(float timestamp, long id)
		{
			Timestamp = timestamp;
			_id = id;
		}

		public int CompareTo(SortableKey other)
		{
			int num = Timestamp.CompareTo(other.Timestamp);
			if (num == 0)
			{
				return _id.CompareTo(other._id);
			}
			return num;
		}
	}

	private readonly IDayNightCycle _dayNightCycle;

	private readonly SortedDictionary<SortableKey, TimeTrigger> _sortedTimeTriggers = new SortedDictionary<SortableKey, TimeTrigger>();

	private readonly Dictionary<TimeTrigger, SortableKey> _timeTriggerKeys = new Dictionary<TimeTrigger, SortableKey>();

	private readonly List<TimeTrigger> _triggersToTrigger = new List<TimeTrigger>();

	private long _nextId;

	public TimeTriggerService(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Tick()
	{
		FindReadyToTrigger();
		Trigger();
	}

	public void Add(TimeTrigger timeTrigger, float triggerTimestamp)
	{
		Remove(timeTrigger);
		SortableKey sortableKey = new SortableKey(triggerTimestamp, _nextId++);
		_sortedTimeTriggers[sortableKey] = timeTrigger;
		_timeTriggerKeys[timeTrigger] = sortableKey;
	}

	public void Remove(TimeTrigger timeTrigger)
	{
		if (_timeTriggerKeys.TryGetValue(timeTrigger, out var value))
		{
			_sortedTimeTriggers.Remove(value);
			_timeTriggerKeys.Remove(timeTrigger);
		}
	}

	private void FindReadyToTrigger()
	{
		float partialDayNumber = _dayNightCycle.PartialDayNumber;
		foreach (var (sortableKey2, item) in _sortedTimeTriggers)
		{
			if (sortableKey2.Timestamp > partialDayNumber)
			{
				break;
			}
			_triggersToTrigger.Add(item);
		}
	}

	private void Trigger()
	{
		foreach (TimeTrigger item in _triggersToTrigger)
		{
			Trigger(item);
		}
		_triggersToTrigger.Clear();
	}

	private void Trigger(TimeTrigger timeTrigger)
	{
		Remove(timeTrigger);
		timeTrigger.Finish();
	}
}
