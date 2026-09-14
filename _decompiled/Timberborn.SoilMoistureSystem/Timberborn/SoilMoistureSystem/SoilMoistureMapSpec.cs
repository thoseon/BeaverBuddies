using Timberborn.BlueprintSystem;

namespace Timberborn.SoilMoistureSystem;

internal record SoilMoistureMapSpec : ComponentSpec
{
	[Serialize]
	public float MaxDesertIntensity { get; init; }

	[Serialize]
	public int DesertMoistureThreshold { get; init; }
}
