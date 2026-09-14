using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlockObjectAccesses;

public record BlockObjectAccessesSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<Vector3Int> BlockingCoordinates { get; init; }

	[Serialize]
	public ImmutableArray<Vector3Int> AllowedCoordinates { get; init; }
}
