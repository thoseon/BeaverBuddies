using Timberborn.BlueprintSystem;

namespace Timberborn.WaterSourceSystem;

internal record WaterSourceContaminationSpec : ComponentSpec
{
	[Serialize]
	public float DefaultContamination { get; init; }
}
