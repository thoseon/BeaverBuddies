using Timberborn.PopulationStatisticsSystem;

namespace Timberborn.DwellingSystem;

internal readonly struct DwellingChangedEventArgs
{
	public DwellingStatistics OldDwellingStatistics { get; }

	public DwellingStatistics NewDwellingStatistics { get; }

	public DwellingChangedEventArgs(DwellingStatistics oldDwellingStatistics, DwellingStatistics newDwellingStatistics)
	{
		OldDwellingStatistics = oldDwellingStatistics;
		NewDwellingStatistics = newDwellingStatistics;
	}
}
