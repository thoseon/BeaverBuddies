using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlockSystemNavigation;

internal record BlockObjectNavMeshEdgeSpec
{
	[Serialize]
	public Vector3Int Start { get; init; }

	[Serialize]
	public Vector3Int End { get; init; }

	[Serialize]
	public bool IsTwoWay { get; init; }
}
