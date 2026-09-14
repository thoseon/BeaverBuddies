namespace Timberborn.HazardousWeatherSystem;

public class HazardousWeatherHistoryData
{
	public string HazardousWeatherId { get; }

	public int Duration { get; }

	public HazardousWeatherHistoryData(string hazardousWeatherId, int duration)
	{
		HazardousWeatherId = hazardousWeatherId;
		Duration = duration;
	}
}
