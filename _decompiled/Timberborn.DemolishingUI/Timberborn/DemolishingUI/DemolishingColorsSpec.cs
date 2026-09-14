using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.DemolishingUI;

internal record DemolishingColorsSpec : ComponentSpec
{
	[Serialize]
	public Color DeletedObjectHighlightColor { get; init; }

	[Serialize]
	public Color DeletedAreaTileColor { get; init; }

	[Serialize]
	public Color DeletedAreaSideColor { get; init; }
}
