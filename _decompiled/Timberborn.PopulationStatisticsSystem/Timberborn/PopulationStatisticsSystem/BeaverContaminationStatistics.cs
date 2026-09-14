namespace Timberborn.PopulationStatisticsSystem;

public readonly struct BeaverContaminationStatistics
{
	public int ContaminatedAdults { get; }

	public int ContaminatedChildren { get; }

	public int Total => ContaminatedAdults + ContaminatedChildren;

	public BeaverContaminationStatistics(int contaminatedAdults, int contaminatedChildren)
	{
		ContaminatedAdults = contaminatedAdults;
		ContaminatedChildren = contaminatedChildren;
	}
}
