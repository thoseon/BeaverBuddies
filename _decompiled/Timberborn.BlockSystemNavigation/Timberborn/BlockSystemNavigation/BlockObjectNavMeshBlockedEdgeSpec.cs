using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlockSystemNavigation;

internal record BlockObjectNavMeshBlockedEdgeSpec
{
	[Serialize]
	public string Group { get; init; }

	[Serialize]
	public Vector3Int Start { get; init; }

	[Serialize]
	public Vector3Int End { get; init; }
}
