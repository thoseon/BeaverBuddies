using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.AreaSelectionSystem;

internal record RectangleBoundsDrawerFactorySpec : ComponentSpec
{
	[Serialize]
	public AssetRef<Mesh> BlockSideMesh0010 { get; init; }

	[Serialize]
	public AssetRef<Mesh> BlockSideMesh0011 { get; init; }

	[Serialize]
	public AssetRef<Mesh> BlockSideMesh0111 { get; init; }

	[Serialize]
	public AssetRef<Mesh> BlockSideMesh1010 { get; init; }

	[Serialize]
	public AssetRef<Mesh> BlockSideMesh1111 { get; init; }

	[Serialize]
	public AssetRef<Material> BlockSideMaterial { get; init; }

	[Serialize]
	public AssetRef<Mesh> BlockBottomMesh { get; init; }

	[Serialize]
	public AssetRef<Material> BlockBottomMaterial { get; init; }
}
