using Timberborn.BlueprintSystem;

namespace Timberborn.WaterSourceSystem;

internal record WaterStrengthSpec : ComponentSpec
{
	[Serialize]
	public float MaxWaterSourceStrength { get; init; }

	[Serialize]
	public float MaxWaterSourceChangePerSecond { get; init; }

	[Serialize]
	public float MinWaterSourceChangeScaler { get; init; }
}
