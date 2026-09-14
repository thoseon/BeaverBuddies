namespace Timberborn.GameDistricts;

public class DistrictSelectedEvent
{
	public DistrictCenter DistrictCenter { get; }

	public DistrictSelectedEvent(DistrictCenter districtCenter)
	{
		DistrictCenter = districtCenter;
	}
}
