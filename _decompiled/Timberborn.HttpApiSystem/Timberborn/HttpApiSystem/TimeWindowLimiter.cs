using System;
using System.Collections.Generic;

namespace Timberborn.HttpApiSystem;

internal class TimeWindowLimiter
{
	private readonly Queue<long> _ticksQueue = new Queue<long>();

	private readonly int _maxOccurrences;

	private readonly long _windowDurationInTicks;

	public TimeWindowLimiter(int maxOccurrences, TimeSpan timeWindow)
	{
		_maxOccurrences = maxOccurrences;
		_windowDurationInTicks = timeWindow.Ticks;
	}

	public bool TryAcquirePermit()
	{
		long ticks = DateTime.Now.Ticks;
		if (_ticksQueue.Count < _maxOccurrences)
		{
			_ticksQueue.Enqueue(ticks);
			return true;
		}
		if (_ticksQueue.Peek() < ticks - _windowDurationInTicks)
		{
			_ticksQueue.Dequeue();
			_ticksQueue.Enqueue(ticks);
			return true;
		}
		return false;
	}
}
