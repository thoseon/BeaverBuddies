using Timberborn.GameCycleSystem;
using Timberborn.HazardousWeatherSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.WeatherSystem;

public class WeatherService : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly TemperateWeatherDurationService _temperateWeatherDurationService;

	private readonly GameCycleService _gameCycleService;

	private readonly HazardousWeatherService _hazardousWeatherService;

	public int HazardousWeatherDuration => _hazardousWeatherService.HazardousWeatherDuration;

	public int TemperateWeatherDuration => _temperateWeatherDurationService.TemperateWeatherDuration;

	public int HazardousWeatherStartCycleDay => TemperateWeatherDuration + 1;

	public bool IsHazardousWeather => _gameCycleService.CycleDay >= HazardousWeatherStartCycleDay;

	public int CycleLengthInDays => TemperateWeatherDuration + HazardousWeatherDuration;

	public WeatherService(EventBus eventBus, TemperateWeatherDurationService temperateWeatherDurationService, GameCycleService gameCycleService, HazardousWeatherService hazardousWeatherService)
	{
		_eventBus = eventBus;
		_temperateWeatherDurationService = temperateWeatherDurationService;
		_gameCycleService = gameCycleService;
		_hazardousWeatherService = hazardousWeatherService;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public bool NextDayIsHazardousWeather()
	{
		int num = _gameCycleService.CycleDay + 1;
		if (num >= HazardousWeatherStartCycleDay)
		{
			return num <= CycleLengthInDays;
		}
		return false;
	}

	[OnEvent]
	public void OnCycleDayStarted(CycleDayStartedEvent cycleDayStartedEvent)
	{
		if (_gameCycleService.CycleDay == HazardousWeatherStartCycleDay)
		{
			_hazardousWeatherService.StartHazardousWeather();
		}
	}

	[OnEvent]
	public void OnCycleEndedEvent(CycleEndedEvent cycleEndedEvent)
	{
		if (HazardousWeatherDuration > 0)
		{
			_hazardousWeatherService.EndHazardousWeather();
		}
	}
}
