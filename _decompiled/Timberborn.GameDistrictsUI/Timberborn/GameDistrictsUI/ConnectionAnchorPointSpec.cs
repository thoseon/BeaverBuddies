using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.GameDistrictsUI;

public record ConnectionAnchorPointSpec : ComponentSpec
{
	[Serialize]
	public Vector3 Position { get; init; }
}
