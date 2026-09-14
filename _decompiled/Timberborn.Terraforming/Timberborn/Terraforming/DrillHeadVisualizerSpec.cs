using Timberborn.BlueprintSystem;

namespace Timberborn.Terraforming;

internal record DrillHeadVisualizerSpec : ComponentSpec
{
	[Serialize]
	public string HeadTransformName { get; init; }

	[Serialize]
	public float HeadOffset { get; init; }
}
