using Timberborn.BlueprintSystem;

namespace Timberborn.SoilContaminationSystem;

internal record SoilContaminationMapSpec : ComponentSpec
{
	[Serialize]
	public float MaxMapContamination { get; init; }

	[Serialize]
	public float ContaminationThreshold { get; init; }
}
