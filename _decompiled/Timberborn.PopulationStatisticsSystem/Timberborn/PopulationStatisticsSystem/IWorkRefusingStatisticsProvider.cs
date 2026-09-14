namespace Timberborn.PopulationStatisticsSystem;

public interface IWorkRefusingStatisticsProvider
{
	WorkRefusingStatistics GetWorkRefusingStatistics(string workerType);
}
