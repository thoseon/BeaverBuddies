namespace Timberborn.PopulationStatisticsSystem;

public readonly struct DwellingStatistics
{
	public int OccupiedBeds { get; }

	public int FreeBeds { get; }

	public DwellingStatistics(int occupiedBeds, int freeBeds)
	{
		OccupiedBeds = occupiedBeds;
		FreeBeds = freeBeds;
	}

	public static DwellingStatistics operator +(DwellingStatistics left, DwellingStatistics right)
	{
		return new DwellingStatistics(left.OccupiedBeds + right.OccupiedBeds, left.FreeBeds + right.FreeBeds);
	}

	public static DwellingStatistics operator -(DwellingStatistics left, DwellingStatistics right)
	{
		return new DwellingStatistics(left.OccupiedBeds - right.OccupiedBeds, left.FreeBeds - right.FreeBeds);
	}
}
