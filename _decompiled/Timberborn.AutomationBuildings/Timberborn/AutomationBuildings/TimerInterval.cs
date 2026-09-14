using System;
using Timberborn.TimeSystem;

namespace Timberborn.AutomationBuildings;

public class TimerInterval
{
	private float? _hours;

	private readonly IDayNightCycle _dayNightCycle;

	public IntervalType Type { get; private set; }

	public int Ticks { get; private set; }

	public TimerInterval(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public float GetTypeTime()
	{
		if (Type == IntervalType.Ticks)
		{
			return Ticks;
		}
		if (_hours.HasValue)
		{
			if (Type != IntervalType.Hours)
			{
				return _hours.Value / 24f;
			}
			return _hours.Value;
		}
		throw new InvalidOperationException("Hours value is not set for non-ticks interval type.");
	}

	public bool TryGetHours(out float hours)
	{
		if (_hours.HasValue)
		{
			hours = _hours.Value;
			return true;
		}
		hours = 0f;
		return false;
	}

	public void DuplicateFrom(TimerInterval source)
	{
		Type = source.Type;
		Ticks = source.Ticks;
		_hours = source._hours;
	}

	public void SetTicks(int ticks)
	{
		Type = IntervalType.Ticks;
		_hours = null;
		Ticks = Math.Max(1, ticks);
	}

	public void SetHours(float hours)
	{
		Type = IntervalType.Hours;
		_hours = ClampHours(hours);
		Ticks = ConvertHoursToTicks(_hours.Value);
	}

	public void SetDays(float days)
	{
		Type = IntervalType.Days;
		_hours = ClampHours(days * 24f);
		Ticks = ConvertHoursToTicks(_hours.Value);
	}

	private float ClampHours(float hours)
	{
		float val = _dayNightCycle.TicksToHours(1);
		return Math.Max(hours, val);
	}

	private int ConvertHoursToTicks(float hours)
	{
		return Math.Max(_dayNightCycle.HoursToTicks(hours), 1);
	}
}
