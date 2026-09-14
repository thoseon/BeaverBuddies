using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Rendering;

public class MarkerDrawerFactory : ILoadableSingleton
{
	private readonly MeshDrawerFactory _meshDrawerFactory;

	private readonly ISpecService _specService;

	private MarkerDrawerFactorySpec _markerDrawerFactorySpec;

	public MarkerDrawerFactory(MeshDrawerFactory meshDrawerFactory, ISpecService specService)
	{
		_meshDrawerFactory = meshDrawerFactory;
		_specService = specService;
	}

	public void Load()
	{
		_markerDrawerFactorySpec = _specService.GetSingleSpec<MarkerDrawerFactorySpec>();
	}

	public MeshDrawer CreateTileDrawer(Color tileColor)
	{
		return _meshDrawerFactory.Create(_markerDrawerFactorySpec.TileMesh, _markerDrawerFactorySpec.TileMaterial.Asset, tileColor);
	}

	public MeshDrawer CreatePrioritizedTileDrawer(Color tileColor)
	{
		return _meshDrawerFactory.Create(_markerDrawerFactorySpec.TileMesh, _markerDrawerFactorySpec.PrioritizedTileMaterial.Asset, tileColor);
	}

	public MeshDrawer CreateTileDrawer()
	{
		return _meshDrawerFactory.Create(_markerDrawerFactorySpec.TileMesh.Asset, _markerDrawerFactorySpec.TileMaterial.Asset);
	}

	public MeshDrawer CreateSmallBlockTileDrawer()
	{
		return _meshDrawerFactory.Create(_markerDrawerFactorySpec.SmallBlockMesh.Asset, _markerDrawerFactorySpec.TileMaterial.Asset);
	}

	public MeshDrawer CreateLargeBlockTileDrawer()
	{
		return _meshDrawerFactory.Create(_markerDrawerFactorySpec.LargeBlockMesh.Asset, _markerDrawerFactorySpec.TileMaterial.Asset);
	}

	public MeshDrawer CreateTerrainTileDrawer()
	{
		return _meshDrawerFactory.Create(_markerDrawerFactorySpec.TerrainBlockMesh.Asset, _markerDrawerFactorySpec.TerrainTileMaterial.Asset);
	}

	public MeshDrawer CreateTopTerrainTileDrawer()
	{
		return _meshDrawerFactory.Create(_markerDrawerFactorySpec.TopTerrainTileMesh.Asset, _markerDrawerFactorySpec.TopTerrainTileMaterial.Asset);
	}

	public MeshDrawer CreateEntranceMarkerDrawer()
	{
		return _meshDrawerFactory.Create(_markerDrawerFactorySpec.EntranceMesh.Asset, _markerDrawerFactorySpec.EntranceMarkerMaterial.Asset);
	}

	public MeshDrawer CreateMechanicalInputMarkerDrawer(Color markerColor)
	{
		return _meshDrawerFactory.Create(_markerDrawerFactorySpec.MechanicalInputMesh, _markerDrawerFactorySpec.MechanicalMarkerMaterial.Asset, markerColor);
	}

	public MeshDrawer CreateMechanicalOutputMarkerDrawer(Color markerColor)
	{
		return _meshDrawerFactory.Create(_markerDrawerFactorySpec.MechanicalOutputMesh, _markerDrawerFactorySpec.MechanicalMarkerMaterial.Asset, markerColor);
	}

	public MeshDrawer CreateArrowMarkerDrawer()
	{
		return _meshDrawerFactory.Create(_markerDrawerFactorySpec.ArrowMesh.Asset, _markerDrawerFactorySpec.ArrowMaterial.Asset);
	}
}
