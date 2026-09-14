namespace Timberborn.HazardousWeatherSystem;

public class HazardousWeatherStartedEvent
{
	public IHazardousWeather HazardousWeather { get; }

	public HazardousWeatherStartedEvent(IHazardousWeather hazardousWeather)
	{
		HazardousWeather = hazardousWeather;
	}
}
