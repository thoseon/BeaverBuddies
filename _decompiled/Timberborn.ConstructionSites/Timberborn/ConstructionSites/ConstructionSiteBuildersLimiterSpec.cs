using Timberborn.BlueprintSystem;

namespace Timberborn.ConstructionSites;

internal record ConstructionSiteBuildersLimiterSpec : ComponentSpec
{
	[Serialize]
	public int MaxAllowedBuilders { get; init; } = 1;
}
