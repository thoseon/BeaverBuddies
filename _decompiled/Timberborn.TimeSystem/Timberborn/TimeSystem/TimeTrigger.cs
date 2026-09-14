using System;
using UnityEngine;

namespace Timberborn.TimeSystem;

internal class TimeTrigger : ITimeTrigger
{
	private readonly IDayNightCycle _dayNightCycle;

	private readonly TimeTriggerService _timeTriggerService;

	private readonly Action _action;

	private readonly float _fullDelayInDays;

	private float _delayLeftInDays;

	private float _resumedTimestamp;

	public bool Finished { get; private set; }

	public bool InProgress { get; private set; }

	public float DaysLeft
	{
		get
		{
			if (!InProgress)
			{
				return _delayLeftInDays;
			}
			return _delayLeftInDays - DaysSinceStart;
		}
	}

	public float Progress => 1f - Mathf.Clamp01(DaysLeft / _fullDelayInDays);

	private float DaysSinceStart => _dayNightCycle.PartialDayNumber - _resumedTimestamp;

	public TimeTrigger(IDayNightCycle dayNightCycle, TimeTriggerService timeTriggerService, Action action, float fullDelayInDays)
	{
		_dayNightCycle = dayNightCycle;
		_timeTriggerService = timeTriggerService;
		_action = action;
		_fullDelayInDays = fullDelayInDays;
		_delayLeftInDays = fullDelayInDays;
	}

	public void Reset()
	{
		Finished = false;
		Pause();
		_delayLeftInDays = _fullDelayInDays;
	}

	public void Resume()
	{
		if (!InProgress && !Finished)
		{
			float partialDayNumber = _dayNightCycle.PartialDayNumber;
			_timeTriggerService.Add(this, partialDayNumber + _delayLeftInDays);
			_resumedTimestamp = partialDayNumber;
			InProgress = true;
		}
	}

	public void Pause()
	{
		if (InProgress)
		{
			_timeTriggerService.Remove(this);
			_delayLeftInDays -= DaysSinceStart;
			InProgress = false;
		}
	}

	public void FastForwardProgress(float progress)
	{
		bool inProgress = InProgress;
		Pause();
		_delayLeftInDays -= _fullDelayInDays * progress;
		if (_delayLeftInDays <= 0f)
		{
			Finish();
		}
		if (inProgress)
		{
			Resume();
		}
	}

	public void Finish()
	{
		if (!Finished)
		{
			InProgress = false;
			Finished = true;
			_delayLeftInDays = 0f;
			_action();
		}
	}
}
