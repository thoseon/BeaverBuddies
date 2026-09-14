using Timberborn.AssetSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ConstructionGuidelines;

internal class TileDrawerFactory : ILoadableSingleton
{
	private readonly IAssetLoader _assetLoader;

	private readonly MeshDrawerFactory _meshDrawerFactory;

	private readonly ISpecService _specService;

	private Mesh _mesh;

	private Material _tilesOnSameLevelMaterial;

	private Material _tilesBelowMaterial;

	private Material _tilesAboveMaterial;

	private Material _footprintTilesMaterial;

	public TileDrawerFactory(IAssetLoader assetLoader, MeshDrawerFactory meshDrawerFactory, ISpecService specService)
	{
		_assetLoader = assetLoader;
		_meshDrawerFactory = meshDrawerFactory;
		_specService = specService;
	}

	public void Load()
	{
		TileDrawerFactorySpec singleSpec = _specService.GetSingleSpec<TileDrawerFactorySpec>();
		_mesh = _assetLoader.Load<Mesh>(singleSpec.MeshResourcePath);
		_tilesOnSameLevelMaterial = _assetLoader.Load<Material>(singleSpec.TilesOnSameLevelMaterialResourcePath);
		_tilesBelowMaterial = _assetLoader.Load<Material>(singleSpec.TilesBelowMaterialResourcePath);
		_tilesAboveMaterial = _assetLoader.Load<Material>(singleSpec.TilesAboveMaterialResourcePath);
		_footprintTilesMaterial = _assetLoader.Load<Material>(singleSpec.FootprintTilesMaterialResourcePath);
	}

	public MeshDrawer CrateSameLevelTileDrawer()
	{
		return _meshDrawerFactory.Create(_mesh, _tilesOnSameLevelMaterial);
	}

	public MeshDrawer CreateBelowTileDrawer()
	{
		return _meshDrawerFactory.Create(_mesh, _tilesBelowMaterial);
	}

	public MeshDrawer CreateAboveTileDrawer()
	{
		return _meshDrawerFactory.Create(_mesh, _tilesAboveMaterial);
	}

	public MeshDrawer CreateFootprintTileDrawer()
	{
		return _meshDrawerFactory.Create(_mesh, _footprintTilesMaterial);
	}
}
