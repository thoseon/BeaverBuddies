using Timberborn.BlueprintSystem;
using Timberborn.HazardousWeatherSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using Timberborn.WeatherSystem;
using UnityEngine;

namespace Timberborn.SkySystem;

public class DayStageCycle : ILoadableSingleton
{
	private readonly IDayNightCycle _dayNightCycle;

	private readonly WeatherService _weatherService;

	private readonly HazardousWeatherService _hazardousWeatherService;

	private readonly ISpecService _specService;

	private float _sunriseSunsetLengthInHours;

	private float _transitionLengthInHours;

	public DayStageCycle(IDayNightCycle dayNightCycle, WeatherService weatherService, HazardousWeatherService hazardousWeatherService, ISpecService specService)
	{
		_dayNightCycle = dayNightCycle;
		_weatherService = weatherService;
		_hazardousWeatherService = hazardousWeatherService;
		_specService = specService;
	}

	public void Load()
	{
		DayStageCycleSpec singleSpec = _specService.GetSingleSpec<DayStageCycleSpec>();
		_sunriseSunsetLengthInHours = singleSpec.SunriseSunsetLengthInHours;
		_transitionLengthInHours = singleSpec.TransitionLengthInHours;
	}

	public DayStageTransition GetCurrentTransition()
	{
		if (!_dayNightCycle.IsDaytime)
		{
			return Transition(TimeOfDay.Nighttime, TimeOfDay.Daytime, DayStage.Sunset, DayStage.Night, DayStage.Sunrise);
		}
		return Transition(TimeOfDay.Daytime, TimeOfDay.Nighttime, DayStage.Sunrise, DayStage.Day, DayStage.Sunset);
	}

	private DayStageTransition Transition(TimeOfDay currentTimeOfDay, TimeOfDay nextTimeOfDay, DayStage dayStage1, DayStage dayStage2, DayStage dayStage3)
	{
		float num = 24f - _dayNightCycle.FluidHoursToNextStartOf(currentTimeOfDay);
		float hoursToNextDayStage = _dayNightCycle.FluidHoursToNextStartOf(nextTimeOfDay);
		if (!(num < _sunriseSunsetLengthInHours))
		{
			return Transition(dayStage2, dayStage3, hoursToNextDayStage);
		}
		return Transition(dayStage1, dayStage2, _sunriseSunsetLengthInHours - num);
	}

	private DayStageTransition Transition(DayStage currentDayStage, DayStage nextDayStage, float hoursToNextDayStage)
	{
		float t = 1f - Mathf.Clamp01(hoursToNextDayStage / _transitionLengthInHours);
		float transitionProgress = Mathf.SmoothStep(0f, 1f, t);
		string text = _hazardousWeatherService.CurrentCycleHazardousWeather?.Id;
		string currentDayStageHazardousWeatherId = (_weatherService.IsHazardousWeather ? text : null);
		string nextDayStageHazardousWeatherId = (((nextDayStage == DayStage.Sunrise) ? _weatherService.NextDayIsHazardousWeather() : _weatherService.IsHazardousWeather) ? text : null);
		return new DayStageTransition(currentDayStage, currentDayStageHazardousWeatherId, nextDayStage, nextDayStageHazardousWeatherId, transitionProgress);
	}
}
