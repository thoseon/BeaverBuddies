using Timberborn.BlueprintSystem;

namespace Timberborn.WaterObjects;

internal record FinishableWaterObstacleSpec : ComponentSpec
{
	[Serialize]
	public float Height { get; init; }
}
