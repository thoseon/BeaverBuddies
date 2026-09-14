using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.ZiplineSystem;

internal record ZiplineTowerSpec : ComponentSpec
{
	[Serialize]
	public Vector3 CableAnchorPoint { get; init; }

	[Serialize]
	public int MaxConnections { get; init; }

	[Serialize]
	public int MaxDistance { get; init; }

	[Serialize]
	public ImmutableArray<Vector3Int> UnobstructedCoordinates { get; init; }
}
