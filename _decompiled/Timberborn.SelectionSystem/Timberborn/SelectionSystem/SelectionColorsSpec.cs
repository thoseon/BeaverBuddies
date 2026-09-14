using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.SelectionSystem;

public record SelectionColorsSpec : ComponentSpec
{
	[Serialize]
	public Color EntitySelection { get; init; }

	[Serialize]
	public Color SelectionToolHighlight { get; init; }
}
