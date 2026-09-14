using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WaterSystemRendering;

internal record WaterMeshSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<GameObject> WaterTile { get; init; }

	[Serialize]
	public AssetRef<Material> OpaqueMaterial { get; init; }

	[Serialize]
	public AssetRef<Material> TransparentMaterial { get; init; }
}
