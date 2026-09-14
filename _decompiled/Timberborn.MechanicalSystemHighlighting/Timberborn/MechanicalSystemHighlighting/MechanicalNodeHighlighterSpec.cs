using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.MechanicalSystemHighlighting;

internal record MechanicalNodeHighlighterSpec : ComponentSpec
{
	[Serialize]
	public Color HighlightColor { get; init; }
}
