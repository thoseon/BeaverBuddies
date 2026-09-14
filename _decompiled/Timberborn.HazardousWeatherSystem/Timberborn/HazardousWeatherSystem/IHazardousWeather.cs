namespace Timberborn.HazardousWeatherSystem;

public interface IHazardousWeather
{
	string Id { get; }

	int GetDurationAtCycle(int cycle);
}
