namespace Timberborn.PopulationStatisticsSystem;

public readonly struct WorkRefusingStatistics
{
	public int RefusingWorkers { get; }

	public int NotRefusingWorkers { get; }

	public WorkRefusingStatistics(int refusingWorkers, int notRefusingWorkers)
	{
		RefusingWorkers = refusingWorkers;
		NotRefusingWorkers = notRefusingWorkers;
	}
}
