using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.EnterableSystem;

internal record RangeEnterableHighlighterSpec : ComponentSpec
{
	[Serialize]
	public Color BuildingInRange { get; init; }
}
