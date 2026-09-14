using Timberborn.BlueprintSystem;

namespace Timberborn.WaterBuildings;

internal record StreamGaugeAnimationControllerSpec : ComponentSpec
{
	[Serialize]
	public string MarkerName { get; init; }

	[Serialize]
	public float MaxHeight { get; init; }
}
