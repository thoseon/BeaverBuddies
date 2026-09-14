using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.MapEditorBrushesUI;

internal record BrushColorSpec : ComponentSpec
{
	[Serialize]
	public Color Neutral { get; init; }

	[Serialize]
	public Color Positive { get; init; }

	[Serialize]
	public Color Negative { get; init; }

	[Serialize]
	public Color Objects { get; init; }
}
