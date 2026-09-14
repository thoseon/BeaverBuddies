using Timberborn.BlueprintSystem;
using Timberborn.GameCycleSystem;
using Timberborn.SingletonSystem;
using Timberborn.WeatherSystem;

namespace Timberborn.HazardousWeatherSystemUI;

public class HazardousWeatherApproachingTimer : ILoadableSingleton, IPostLoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly WeatherService _weatherService;

	private readonly GameCycleService _gameCycleService;

	private readonly ISpecService _specService;

	private HazardousWeatherUISpec _spec;

	private bool TooCloseToNotify => DaysToHazardousWeather < _spec.MaxDayProgressLeftToNotify;

	private float DaysToHazardousWeather => (float)_weatherService.HazardousWeatherStartCycleDay - _gameCycleService.PartialCycleDay;

	public HazardousWeatherApproachingTimer(EventBus eventBus, WeatherService weatherService, GameCycleService gameCycleService, ISpecService specService)
	{
		_eventBus = eventBus;
		_weatherService = weatherService;
		_gameCycleService = gameCycleService;
		_specService = specService;
	}

	public void Load()
	{
		_spec = _specService.GetSingleSpec<HazardousWeatherUISpec>();
		_eventBus.Register(this);
	}

	public void PostLoad()
	{
		if (GetProgress() > 0f && !TooCloseToNotify)
		{
			NotifyHazardousWeatherApproaching();
		}
	}

	public float GetProgress()
	{
		if (_weatherService.HazardousWeatherDuration <= 0)
		{
			return 0f;
		}
		return 1f - DaysToHazardousWeather / (float)_spec.ApproachingNotificationDays;
	}

	[OnEvent]
	public void OnCycleDayStarted(CycleDayStartedEvent cycleDayStartedEvent)
	{
		if (_gameCycleService.CycleDay == _weatherService.TemperateWeatherDuration - _spec.ApproachingNotificationDays + 1)
		{
			NotifyHazardousWeatherApproaching();
		}
	}

	private void NotifyHazardousWeatherApproaching()
	{
		if (_weatherService.HazardousWeatherDuration > 0)
		{
			_eventBus.Post(new HazardousWeatherApproachingEvent());
		}
	}
}
