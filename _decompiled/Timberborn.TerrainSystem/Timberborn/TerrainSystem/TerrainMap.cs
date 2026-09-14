using System;
using System.Linq;
using Timberborn.Common;
using Timberborn.ErrorReporting;
using Timberborn.MapIndexSystem;
using Timberborn.MapStateSystem;
using Timberborn.PackedListSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.TerrainSystem;

public class TerrainMap : ISaveableSingleton, ILoadableSingleton
{
	private static readonly string TerrainCutOffIssueLocKey = "LoadingIssue.TerrainCutOffIssue";

	private static readonly SingletonKey TerrainMapKey = new SingletonKey("TerrainMap");

	private static readonly PropertyKey<PackedList<bool>> VoxelsKey = new PropertyKey<PackedList<bool>>("Voxels");

	private static readonly int NewMapHeight = 4;

	private static readonly int DefaultHeight = -1;

	private readonly ISingletonLoader _singletonLoader;

	private readonly MapIndexService _mapIndexService;

	private readonly IntPackedListSerializer _intPackedListSerializer;

	private readonly BoolPackedListSerializer _boolPackedListSerializer;

	private readonly MapSize _mapSize;

	private readonly ILoadingIssueService _loadingIssueService;

	private bool[] _terrainVoxels;

	public event EventHandler<Vector3Int> TerrainAdded;

	public event EventHandler<Vector3Int> TerrainRemoved;

	public TerrainMap(ISingletonLoader singletonLoader, MapIndexService mapSerializer, IntPackedListSerializer intPackedListSerializer, BoolPackedListSerializer boolPackedListSerializer, MapSize mapSize, ILoadingIssueService loadingIssueService)
	{
		_singletonLoader = singletonLoader;
		_mapIndexService = mapSerializer;
		_intPackedListSerializer = intPackedListSerializer;
		_boolPackedListSerializer = boolPackedListSerializer;
		_mapSize = mapSize;
		_loadingIssueService = loadingIssueService;
	}

	public bool TryGetRelativeHeight(Vector3Int coordinates, out int relativeHeight)
	{
		Vector2Int coordinates2 = coordinates.XY();
		if (Contains(coordinates2))
		{
			Vector3Int coordinates3 = ClampToTerrain(coordinates);
			int num = _mapIndexService.CoordinatesToIndex3D(coordinates3);
			if (_terrainVoxels[num])
			{
				relativeHeight = GetDistanceToTerrainTop(num, coordinates3);
				return true;
			}
			int num2 = Math.Max(0, coordinates.z - _mapSize.TerrainSize.z);
			relativeHeight = -GetDistanceToTerrainBelow(num, coordinates3) - num2;
			return true;
		}
		relativeHeight = DefaultHeight;
		return false;
	}

	public int GetTerrainHeightBelow(Vector3Int coordinates)
	{
		Vector3Int coordinates2 = ClampToTerrain(coordinates);
		int num = _mapIndexService.CoordinatesToIndex3D(coordinates2);
		int z = coordinates2.z;
		for (int num2 = coordinates2.z; num2 >= 0; num2--)
		{
			int num3 = z - num2;
			int num4 = num - num3 * _mapIndexService.VerticalStride;
			if (_terrainVoxels[num4])
			{
				return num2 + 1;
			}
		}
		return 0;
	}

	public void SetTerrain(Vector3Int coordinates)
	{
		int num = _mapIndexService.CoordinatesToIndex3D(coordinates);
		_terrainVoxels[num] = true;
		TerrainAdded?.Invoke(this, coordinates);
	}

	public void UnsetTerrain(Vector3Int coordinates)
	{
		int num = _mapIndexService.CoordinatesToIndex3D(coordinates);
		_terrainVoxels[num] = false;
		TerrainRemoved?.Invoke(this, coordinates);
	}

	public bool UnsafeIsTerrainVoxel(int index)
	{
		return _terrainVoxels[index];
	}

	public bool IsTerrainVoxel(Vector3Int coordinates)
	{
		if (Contains(coordinates.XY()))
		{
			if (coordinates.z >= 0)
			{
				if (coordinates.z < _mapSize.TerrainSize.z)
				{
					return _terrainVoxels[_mapIndexService.CoordinatesToIndex3D(coordinates)];
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(TerrainMapKey).Set(VoxelsKey, _mapIndexService.Pack3D(_terrainVoxels), _boolPackedListSerializer);
	}

	[BackwardCompatible(2024, 8, 29, Compatibility.Map)]
	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(TerrainMapKey, out var objectLoader))
		{
			PropertyKey<PackedList<int>> key = new PropertyKey<PackedList<int>>("Heights");
			if (objectLoader.Has(key))
			{
				BackwardCompatibleLoad(objectLoader.Get(key, _intPackedListSerializer));
				return;
			}
			PackedList<bool> packedList = objectLoader.Get(VoxelsKey, _boolPackedListSerializer);
			_terrainVoxels = _mapIndexService.Unpack3D(GetTerrainData(packedList.Array));
		}
		else
		{
			InitializeHeights();
		}
	}

	private bool Contains(Vector2Int coordinates)
	{
		return Sizing.SizeContains(_mapSize.TerrainSize2D, coordinates);
	}

	private int GetDistanceToTerrainTop(int startIndex, Vector3Int coordinates)
	{
		int num = 0;
		for (int i = 0; i < _mapSize.TerrainSize.z - coordinates.z; i++)
		{
			if (_terrainVoxels[startIndex + _mapIndexService.VerticalStride * i])
			{
				num++;
				continue;
			}
			return num;
		}
		return num;
	}

	private int GetDistanceToTerrainBelow(int startIndex, Vector3Int coordinates)
	{
		int num = 0;
		for (int i = 1; i <= coordinates.z; i++)
		{
			if (!_terrainVoxels[startIndex - _mapIndexService.VerticalStride * i])
			{
				num++;
				continue;
			}
			return num;
		}
		return num;
	}

	private Vector3Int ClampToTerrain(Vector3Int position)
	{
		return new Vector3Int(position.x, position.y, Math.Clamp(position.z, 0, _mapSize.TerrainSize.z - 1));
	}

	private void BackwardCompatibleLoad(PackedList<int> packedHeightsList)
	{
		int[] array = _mapIndexService.Unpack2DHeightData(packedHeightsList);
		if (array.Any((int height) => height > _mapIndexService.TerrainSize.z))
		{
			throw new InvalidOperationException("Loaded map has heights exceeding map size");
		}
		int x = _mapIndexService.TerrainSize.x;
		_terrainVoxels = new bool[_mapIndexService.MaxSize3D];
		for (int num = 0; num < array.Length; num++)
		{
			int num2 = array[num];
			int z = _mapIndexService.TerrainSize.z;
			for (int num3 = 0; num3 < z; num3++)
			{
				if (num3 < num2)
				{
					SetTerrain(new Vector3Int(num % x, num / x, num3));
				}
			}
		}
	}

	private bool[] GetTerrainData(bool[] currentTerrainData)
	{
		int num = _mapSize.TerrainSize.x * _mapSize.TerrainSize.y;
		if (currentTerrainData.Length / num <= _mapSize.TerrainSize.z)
		{
			return currentTerrainData;
		}
		_loadingIssueService.AddIssue("Terrain data height exceeds map size, truncating to fit.", TerrainCutOffIssueLocKey);
		bool[] array = new bool[_mapSize.TerrainSize.z * num];
		for (int i = 0; i < num * _mapSize.MaxGameTerrainHeight; i++)
		{
			array[i] = currentTerrainData[i];
		}
		return array;
	}

	private void InitializeHeights()
	{
		_terrainVoxels = new bool[_mapIndexService.MaxSize3D];
		foreach (int item in _mapIndexService.Indices2D)
		{
			for (int i = 0; i < NewMapHeight; i++)
			{
				_terrainVoxels[item + i * _mapIndexService.VerticalStride] = true;
			}
		}
	}
}
