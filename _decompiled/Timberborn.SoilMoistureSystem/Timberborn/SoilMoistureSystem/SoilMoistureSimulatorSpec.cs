using Timberborn.BlueprintSystem;

namespace Timberborn.SoilMoistureSystem;

internal record SoilMoistureSimulatorSpec : ComponentSpec
{
	[Serialize]
	public float MinimumWaterContamination { get; init; }

	[Serialize]
	public float MaximumWaterContamination { get; init; }

	[Serialize]
	public float MoistureDecayRate { get; init; }

	[Serialize]
	public float MoistureSpreadingRate { get; init; }

	[Serialize]
	public int VerticalSpreadCostMultiplier { get; init; }

	[Serialize]
	public int MaxClusterSaturation { get; init; }

	[Serialize]
	public float QuadraticEvaporationCoefficient { get; init; }

	[Serialize]
	public float LinearQuadraticCoefficient { get; init; }

	[Serialize]
	public float ConstantQuadraticCoefficient { get; init; }

	[Serialize]
	public int MaxEvaporationSaturation { get; init; }
}
