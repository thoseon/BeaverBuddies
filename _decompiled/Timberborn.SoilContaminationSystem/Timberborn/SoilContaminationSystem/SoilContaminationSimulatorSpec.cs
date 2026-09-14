using Timberborn.BlueprintSystem;

namespace Timberborn.SoilContaminationSystem;

internal record SoilContaminationSimulatorSpec : ComponentSpec
{
	[Serialize]
	public int MaxRangeFromSource { get; init; }

	[Serialize]
	public float VerticalSpreadCostMultiplier { get; init; }

	[Serialize]
	public float ContaminationSpreadingRate { get; init; }

	[Serialize]
	public float ContaminationDecayRate { get; init; }

	[Serialize]
	public float ContaminationPositiveEqualizationRate { get; init; }

	[Serialize]
	public float ContaminationNegativeEqualizationRate { get; init; }

	[Serialize]
	public float MinimumWaterContamination { get; init; }

	[Serialize]
	public float ContaminationThreshold { get; init; }
}
