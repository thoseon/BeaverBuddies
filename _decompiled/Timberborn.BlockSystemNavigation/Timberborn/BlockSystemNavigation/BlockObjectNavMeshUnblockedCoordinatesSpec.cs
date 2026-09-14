using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlockSystemNavigation;

internal record BlockObjectNavMeshUnblockedCoordinatesSpec
{
	[Serialize]
	public string Group { get; init; }

	[Serialize]
	public Vector3Int Coordinates { get; init; }
}
