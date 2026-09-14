using Timberborn.BlueprintSystem;

namespace Timberborn.BlockObstacles;

internal record LayeredBlockObstacleVisualizerSpec : ComponentSpec
{
	[Serialize]
	public string PositionTransformName { get; init; }

	[Serialize]
	public string ScaleTransformName { get; init; }
}
