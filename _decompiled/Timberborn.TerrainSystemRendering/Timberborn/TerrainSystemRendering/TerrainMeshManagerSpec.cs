using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.TerrainSystemRendering;

internal record TerrainMeshManagerSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<GameObject> TerrainTilePrefab { get; init; }

	[Serialize]
	public AssetRef<GameObject> LayerToolTopMeshPrefab { get; init; }
}
