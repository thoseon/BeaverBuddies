using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.NeedSpecs;

internal record NeedSpecFormatterSpec : ComponentSpec
{
	[Serialize]
	public Color PositiveHighlightColor { get; init; }

	[Serialize]
	public Color NegativeHighlightColor { get; init; }
}
