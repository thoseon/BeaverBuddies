using System;
using Timberborn.TimeSystem;

namespace Timberborn.AutomationBuildings;

public class TimerIntervalFactory
{
	private readonly IDayNightCycle _dayNightCycle;

	public TimerIntervalFactory(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public TimerInterval CreateFromTicks(int ticks)
	{
		TimerInterval timerInterval = new TimerInterval(_dayNightCycle);
		timerInterval.SetTicks(ticks);
		return timerInterval;
	}

	public TimerInterval CreateFromHours(float hours, IntervalType intervalType)
	{
		TimerInterval timerInterval = new TimerInterval(_dayNightCycle);
		switch (intervalType)
		{
		case IntervalType.Days:
			timerInterval.SetDays(hours / 24f);
			break;
		case IntervalType.Hours:
			timerInterval.SetHours(hours);
			break;
		default:
			throw new ArgumentOutOfRangeException("intervalType", intervalType, null);
		}
		return timerInterval;
	}
}
