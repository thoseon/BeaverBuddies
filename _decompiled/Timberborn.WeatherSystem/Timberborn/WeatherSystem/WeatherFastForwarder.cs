using Timberborn.GameCycleSystem;
using Timberborn.TimeSystem;

namespace Timberborn.WeatherSystem;

public class WeatherFastForwarder
{
	private readonly WeatherService _weatherService;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly GameCycleService _gameCycleService;

	public WeatherFastForwarder(WeatherService weatherService, IDayNightCycle dayNightCycle, GameCycleService gameCycleService)
	{
		_weatherService = weatherService;
		_dayNightCycle = dayNightCycle;
		_gameCycleService = gameCycleService;
	}

	public void JumpToNextSeason()
	{
		if (_weatherService.IsHazardousWeather)
		{
			int daysToSkip = _weatherService.HazardousWeatherStartCycleDay + _weatherService.HazardousWeatherDuration - _gameCycleService.CycleDay;
			SkipDays(daysToSkip);
		}
		else
		{
			int daysToSkip2 = _weatherService.HazardousWeatherStartCycleDay - _gameCycleService.CycleDay;
			SkipDays(daysToSkip2);
		}
	}

	private void SkipDays(int daysToSkip)
	{
		for (int i = 0; i < daysToSkip; i++)
		{
			_dayNightCycle.SetTimeToNextDay();
		}
	}
}
