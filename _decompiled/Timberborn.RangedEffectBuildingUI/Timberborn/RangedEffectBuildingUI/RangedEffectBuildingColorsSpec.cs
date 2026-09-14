using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.RangedEffectBuildingUI;

internal record RangedEffectBuildingColorsSpec : ComponentSpec
{
	[Serialize]
	public Color BuildingRangeTile { get; init; }

	[Serialize]
	public Color BuildingRangeObject { get; init; }
}
