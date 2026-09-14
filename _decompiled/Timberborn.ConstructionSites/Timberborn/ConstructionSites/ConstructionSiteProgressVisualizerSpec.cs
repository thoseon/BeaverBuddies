using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.ConstructionSites;

internal record ConstructionSiteProgressVisualizerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<float> ProgressThresholds { get; init; }
}
