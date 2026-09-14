using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.Rendering;

internal record MarkerDrawerFactorySpec : ComponentSpec
{
	[Serialize]
	public AssetRef<Mesh> TileMesh { get; init; }

	[Serialize]
	public AssetRef<Mesh> SmallBlockMesh { get; init; }

	[Serialize]
	public AssetRef<Mesh> LargeBlockMesh { get; init; }

	[Serialize]
	public AssetRef<Mesh> TerrainBlockMesh { get; init; }

	[Serialize]
	public AssetRef<Mesh> TopTerrainTileMesh { get; set; }

	[Serialize]
	public AssetRef<Material> TileMaterial { get; init; }

	[Serialize]
	public AssetRef<Material> TerrainTileMaterial { get; init; }

	[Serialize]
	public AssetRef<Material> TopTerrainTileMaterial { get; init; }

	[Serialize]
	public AssetRef<Material> PrioritizedTileMaterial { get; init; }

	[Serialize]
	public AssetRef<Mesh> EntranceMesh { get; init; }

	[Serialize]
	public AssetRef<Material> EntranceMarkerMaterial { get; init; }

	[Serialize]
	public AssetRef<Mesh> MechanicalInputMesh { get; init; }

	[Serialize]
	public AssetRef<Mesh> MechanicalOutputMesh { get; init; }

	[Serialize]
	public AssetRef<Material> MechanicalMarkerMaterial { get; init; }

	[Serialize]
	public AssetRef<Mesh> ArrowMesh { get; init; }

	[Serialize]
	public AssetRef<Material> ArrowMaterial { get; init; }
}
