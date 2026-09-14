namespace Timberborn.HazardousWeatherSystem;

public class HazardousWeatherEndedEvent
{
	public IHazardousWeather HazardousWeather { get; }

	public HazardousWeatherEndedEvent(IHazardousWeather hazardousWeather)
	{
		HazardousWeather = hazardousWeather;
	}
}
