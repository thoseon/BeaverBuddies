using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.PrioritySystemUI;

internal record PriorityColorsSpec : ComponentSpec
{
	[Serialize]
	public Color HighlightVeryLow { get; init; }

	[Serialize]
	public Color HighlightLow { get; init; }

	[Serialize]
	public Color HighlightNormal { get; init; }

	[Serialize]
	public Color HighlightHigh { get; init; }

	[Serialize]
	public Color HighlightVeryHigh { get; init; }

	[Serialize]
	public Color ButtonVeryLow { get; init; }

	[Serialize]
	public Color ButtonLow { get; init; }

	[Serialize]
	public Color ButtonNormal { get; init; }

	[Serialize]
	public Color ButtonHigh { get; init; }

	[Serialize]
	public Color ButtonVeryHigh { get; init; }
}
