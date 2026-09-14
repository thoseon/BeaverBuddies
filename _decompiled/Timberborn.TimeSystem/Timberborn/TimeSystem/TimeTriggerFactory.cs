using System;

namespace Timberborn.TimeSystem;

internal class TimeTriggerFactory : ITimeTriggerFactory
{
	private readonly IDayNightCycle _dayNightCycle;

	private readonly TimeTriggerService _timeTriggerService;

	public TimeTriggerFactory(IDayNightCycle dayNightCycle, TimeTriggerService timeTriggerService)
	{
		_dayNightCycle = dayNightCycle;
		_timeTriggerService = timeTriggerService;
	}

	public ITimeTrigger Create(Action action, float delayInDays)
	{
		return new TimeTrigger(_dayNightCycle, _timeTriggerService, action, delayInDays);
	}
}
