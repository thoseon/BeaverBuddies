namespace Timberborn.HazardousWeatherSystem;

public class HazardousWeatherSelectedEvent
{
	public IHazardousWeather SelectedWeather { get; }

	public int Duration { get; }

	public HazardousWeatherSelectedEvent(IHazardousWeather selectedWeather, int duration)
	{
		SelectedWeather = selectedWeather;
		Duration = duration;
	}
}
