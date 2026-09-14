using Timberborn.PopulationStatisticsSystem;

namespace Timberborn.PopulationWorkStatistics;

internal readonly struct WorkerCountChangedEventArgs
{
	public EmploymentStatistics OldEmploymentStatistics { get; }

	public EmploymentStatistics NewEmploymentStatistics { get; }

	public WorkerCountChangedEventArgs(EmploymentStatistics oldEmploymentStatistics, EmploymentStatistics newEmploymentStatistics)
	{
		OldEmploymentStatistics = oldEmploymentStatistics;
		NewEmploymentStatistics = newEmploymentStatistics;
	}
}
