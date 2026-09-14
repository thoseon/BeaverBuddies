using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.DuplicationSystemUI;

internal record DuplicationSystemColorsSpec : ComponentSpec
{
	[Serialize]
	public Color SourceColor { get; init; }

	[Serialize]
	public Color TargetColor { get; init; }
}
