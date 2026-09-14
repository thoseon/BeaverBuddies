using Timberborn.BlueprintSystem;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.Rendering;

public class AreaTileDrawerFactory : ILoadableSingleton
{
	private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");

	private readonly MapSize _mapSize;

	private readonly ISpecService _specService;

	private AreaTileDrawerFactorySpec _areaTileDrawerFactorySpec;

	public AreaTileDrawerFactory(MapSize mapSize, ISpecService specService)
	{
		_mapSize = mapSize;
		_specService = specService;
	}

	public void Load()
	{
		_areaTileDrawerFactorySpec = _specService.GetSingleSpec<AreaTileDrawerFactorySpec>();
	}

	public AreaTileDrawer Create(Color color, GameObject parent)
	{
		GameObject gameObject = new GameObject(parent.name + "AreaTileDrawer");
		gameObject.transform.parent = parent.transform;
		Material material = new Material(_areaTileDrawerFactorySpec.TileMaterial.Asset);
		material.SetColor(ColorProperty, color);
		Vector2Int tileCount = WorldTiling.TileCount2D(_mapSize.TerrainSize.x, _mapSize.TerrainSize.y);
		return new AreaTileDrawer(_areaTileDrawerFactorySpec.TileMesh.Asset, material, tileCount, gameObject);
	}
}
