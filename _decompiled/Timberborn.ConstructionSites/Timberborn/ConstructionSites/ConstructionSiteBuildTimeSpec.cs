using Timberborn.BlueprintSystem;

namespace Timberborn.ConstructionSites;

internal record ConstructionSiteBuildTimeSpec : ComponentSpec
{
	[Serialize]
	public float ConstructionTimeInHours { get; init; } = 1f;
}
