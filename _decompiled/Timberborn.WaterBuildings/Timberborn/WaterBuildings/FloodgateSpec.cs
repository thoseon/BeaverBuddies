using Timberborn.BlueprintSystem;

namespace Timberborn.WaterBuildings;

internal record FloodgateSpec : ComponentSpec
{
	[Serialize]
	public int MaxHeight { get; init; }
}
