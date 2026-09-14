using Timberborn.BlueprintSystem;

namespace Timberborn.WaterBuildings;

internal record FloodgateAnimationControllerSpec : ComponentSpec
{
	[Serialize]
	public string GateName { get; init; }
}
