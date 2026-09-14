using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WaterBrushesUI;

internal record WaterHeightBrushSpec : ComponentSpec
{
	[Serialize]
	public Color AddingTileColor { get; init; }

	[Serialize]
	public Color RemovingTileColor { get; init; }

	[Serialize]
	public Color ContaminatedTileColor { get; init; }
}
