using Timberborn.BlueprintSystem;

namespace Timberborn.ScienceSystem;

internal record ScienceNeedingBuildingSpec : ComponentSpec
{
	[Serialize]
	public int ScienceUsedPerHour { get; init; }
}
