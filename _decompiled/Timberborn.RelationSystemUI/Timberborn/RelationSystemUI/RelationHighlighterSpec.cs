using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.RelationSystemUI;

internal record RelationHighlighterSpec : ComponentSpec
{
	[Serialize]
	public Color RelationSelection { get; init; }
}
