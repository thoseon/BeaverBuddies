using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.GameDistrictsUI;

internal record DistrictConnectionDrawingServiceSpec : ComponentSpec
{
	[Serialize]
	public Color ConnectionHighlight { get; init; }
}
