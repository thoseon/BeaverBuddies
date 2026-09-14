using Timberborn.BlueprintSystem;

namespace Timberborn.WaterBuildings;

public record WaterInputPipeSpec : ComponentSpec
{
	[Serialize]
	public string PipeSegmentPrefabPath { get; init; }

	[Serialize]
	public string PipeParentName { get; init; }

	[Serialize]
	public int MaxDepth { get; init; }
}
