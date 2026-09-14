using Timberborn.BlueprintSystem;

namespace Timberborn.TimeSystem;

internal record DayNightCycleSpec : ComponentSpec
{
	[Serialize]
	public int HoursPassedOnNewGame { get; init; }

	[Serialize]
	public int ConfiguredDayLengthInTicks { get; init; }

	[Serialize]
	public int ConfiguredDaytimeLengthInHours { get; init; }
}
