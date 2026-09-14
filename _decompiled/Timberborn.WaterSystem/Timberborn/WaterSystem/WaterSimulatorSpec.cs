using Timberborn.BlueprintSystem;

namespace Timberborn.WaterSystem;

internal record WaterSimulatorSpec : ComponentSpec
{
	[Serialize]
	public float WaterFlowFactor { get; init; }

	[Serialize]
	public float WaterSpillThreshold { get; init; }

	[Serialize]
	public float NormalEvaporationSpeed { get; init; }

	[Serialize]
	public float FastEvaporationDepthThreshold { get; init; }

	[Serialize]
	public float FastEvaporationSpeed { get; init; }

	[Serialize]
	public float OutflowBalancingScaler { get; init; }

	[Serialize]
	public float SoftDamOffset { get; init; }

	[Serialize]
	public float HardDamOffset { get; init; }

	[Serialize]
	public float HardDamSmoothingFactor { get; init; }

	[Serialize]
	public float MinHardDamSmoothing { get; init; }

	[Serialize]
	public float MaxHardDamSmoothing { get; init; }

	[Serialize]
	public float MaxHardDamDecrease { get; init; }

	[Serialize]
	public float OverflowPressureFactor { get; init; }

	[Serialize]
	public float DiffusionOutflowLimit { get; init; }

	[Serialize]
	public float DiffusionDepthLimit { get; init; }

	[Serialize]
	public float DiffusionRate { get; init; }

	[Serialize]
	public float MaxWaterContamination { get; init; }
}
