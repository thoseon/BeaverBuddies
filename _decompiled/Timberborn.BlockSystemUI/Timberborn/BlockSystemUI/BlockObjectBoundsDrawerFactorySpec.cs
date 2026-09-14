using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlockSystemUI;

internal record BlockObjectBoundsDrawerFactorySpec : ComponentSpec
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
	public AssetRef<Material> Material { get; init; }
}
