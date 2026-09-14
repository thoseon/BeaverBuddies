namespace Timberborn.PopulationStatisticsSystem;

public interface IEmploymentStatisticsProvider
{
	EmploymentStatistics GetEmploymentStatistics(string workerType);
}
