using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.PathSystem;

public record PathSpec : ComponentSpec
{
	[Serialize]
	public Vector3Int MainPathCoordinates { get; init; }
}
