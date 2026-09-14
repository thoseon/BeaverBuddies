using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BuilderPrioritySystemUI;

internal record BuilderPriorityToolSpec : ComponentSpec
{
	[Serialize]
	public Color PriorityHighlightColor { get; init; }

	[Serialize]
	public Color PriorityActionColor { get; init; }

	[Serialize]
	public Color PriorityTileColor { get; init; }

	[Serialize]
	public Color PrioritySideColor { get; init; }
}
