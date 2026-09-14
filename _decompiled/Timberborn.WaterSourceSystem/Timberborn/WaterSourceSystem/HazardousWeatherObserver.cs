using Timberborn.BaseComponentSystem;
using Timberborn.HazardousWeatherSystem;
using Timberborn.WeatherSystem;

namespace Timberborn.WaterSourceSystem;

public class HazardousWeatherObserver : BaseComponent
{
	private readonly HazardousWeatherService _hazardousWeatherService;

	private readonly WeatherService _weatherService;

	public bool IsBadtideWeather
	{
		get
		{
			if (_weatherService.IsHazardousWeather)
			{
				return _hazardousWeatherService.CurrentCycleHazardousWeather is BadtideWeather;
			}
			return false;
		}
	}

	public bool IsDroughtWeather
	{
		get
		{
			if (_weatherService.IsHazardousWeather)
			{
				return _hazardousWeatherService.CurrentCycleHazardousWeather is DroughtWeather;
			}
			return false;
		}
	}

	public HazardousWeatherObserver(HazardousWeatherService hazardousWeatherService, WeatherService weatherService)
	{
		_hazardousWeatherService = hazardousWeatherService;
		_weatherService = weatherService;
	}
}
