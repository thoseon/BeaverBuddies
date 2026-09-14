using Timberborn.BlueprintSystem;

namespace Timberborn.BlockSystemUI;

internal record UndergroundDepthDescriberSpec : ComponentSpec
{
	[Serialize]
	public int Depth { get; init; }
}
