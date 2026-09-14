using Timberborn.BlueprintSystem;

namespace Timberborn.WaterBuildings;

internal record StreamGaugeSpec : ComponentSpec
{
	[Serialize]
	public float MaxWaterLevel { get; init; }
}
