using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.MapStateSystem;

public class MapSize : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey MapSizeKey = new SingletonKey("MapSize");

	private static readonly PropertyKey<Vector2Int> SizeKey = new PropertyKey<Vector2Int>("Size");

	private readonly ISingletonLoader _singletonLoader;

	private readonly ISpecService _specService;

	private MapSizeSpec _mapSizeSpec;

	public Vector3Int TerrainSize { get; private set; }

	public Vector3Int TotalSize { get; private set; }

	public Vector2Int TerrainSize2D { get; private set; }

	public int MaxGameTerrainHeight => _mapSizeSpec.MaxGameTerrainHeight;

	public int MaxMapEditorTerrainHeight => _mapSizeSpec.MaxMapEditorTerrainHeight;

	public int MaxHeightAboveTerrain => _mapSizeSpec.MaxHeightAboveTerrain;

	public MapSize(ISingletonLoader singletonLoader, ISpecService specService)
	{
		_singletonLoader = singletonLoader;
		_specService = specService;
	}

	public static MapSize NewMap(Vector2Int size)
	{
		return new MapSize(null, null)
		{
			TerrainSize2D = size
		};
	}

	public void Load()
	{
		IObjectLoader singleton = _singletonLoader.GetSingleton(MapSizeKey);
		_mapSizeSpec = _specService.GetSingleSpec<MapSizeSpec>();
		Initialize(singleton.Get(SizeKey));
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(MapSizeKey).Set(SizeKey, TerrainSize2D);
	}

	public bool ContainsInTerrain(Vector2Int coordinates)
	{
		return Sizing.SizeContains(TerrainSize, coordinates);
	}

	public bool ContainsInTotal(Vector3Int coordinates)
	{
		return Sizing.SizeContains(TotalSize, coordinates);
	}

	private void Initialize(Vector2Int size)
	{
		TerrainSize2D = size;
		TerrainSize = size.ToVector3Int(_mapSizeSpec.MaxGameTerrainHeight + 1);
		TotalSize = TerrainSize + new Vector3Int(0, 0, _mapSizeSpec.MaxHeightAboveTerrain);
	}
}
