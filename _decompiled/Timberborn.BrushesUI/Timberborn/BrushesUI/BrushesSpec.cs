using Timberborn.BlueprintSystem;

namespace Timberborn.BrushesUI;

internal record BrushesSpec : ComponentSpec
{
	[Serialize]
	public int MaxBrushSize { get; init; }
}
