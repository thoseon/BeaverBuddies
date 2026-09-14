using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlockObstacles;

internal record LayeredBlockObstacleSpec : ComponentSpec
{
	[Serialize]
	public Vector2Int LayerSize { get; init; }

	[Serialize]
	public Vector3 AnchorPosition { get; init; }

	[Serialize]
	public int BlockCreationOffset { get; init; }
}
