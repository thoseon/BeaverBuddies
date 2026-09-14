using Timberborn.BlueprintSystem;

namespace Timberborn.WaterBuildings;

internal record WaterMoverSpec : ComponentSpec
{
	[Serialize]
	public float WaterPerSecond { get; init; }
}
