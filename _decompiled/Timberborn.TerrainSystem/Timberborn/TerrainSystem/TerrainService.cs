using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.MapIndexSystem;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.TerrainSystem;

internal class TerrainService : ILoadableSingleton, ITerrainService, IPostLoadableSingleton
{
	private readonly MapSize _mapSize;

	private readonly TerrainMap _terrainMap;

	private readonly ColumnTerrainMap _columnTerrainMap;

	private readonly MapIndexService _mapIndexService;

	private readonly List<TerrainHeightChange> _terrainHeightChanges = new List<TerrainHeightChange>();

	private readonly Dictionary<Vector2Int, List<int>> _tileChanges = new Dictionary<Vector2Int, List<int>>();

	private TerrainPropertyMap<bool> _fieldMap;

	private TerrainPropertyMap<int> _cutoutMap;

	public int MaxTerrainHeight { get; private set; }

	public int MinTerrainHeight { get; private set; }

	public Vector3Int Size => _mapSize.TerrainSize;

	public event EventHandler<TerrainHeightChangeEventArgs> PreTerrainHeightChanged;

	public event EventHandler<TerrainHeightChangeEventArgs> TerrainHeightChanged;

	public event EventHandler MinMaxTerrainHeightChanged;

	public event EventHandler<Vector3Int> FieldOrCutoutChanged;

	public TerrainService(MapSize mapSize, TerrainMap terrainMap, ColumnTerrainMap columnTerrainMap, MapIndexService mapIndexService)
	{
		_mapSize = mapSize;
		_terrainMap = terrainMap;
		_columnTerrainMap = columnTerrainMap;
		_mapIndexService = mapIndexService;
	}

	public void Load()
	{
		_fieldMap = new TerrainPropertyMap<bool>(Size);
		_cutoutMap = new TerrainPropertyMap<int>(Size);
	}

	public void PostLoad()
	{
		CalculateMinAndMaxHeight();
	}

	public int GetTerrainHeight(Vector3Int coordinates)
	{
		if (TryGetRelativeHeight(coordinates, out var relativeHeight))
		{
			return coordinates.z + relativeHeight;
		}
		return 0;
	}

	public bool TryGetRelativeHeight(Vector3Int coordinates, out int relativeHeight)
	{
		return _terrainMap.TryGetRelativeHeight(coordinates, out relativeHeight);
	}

	public int GetTerrainHeightBelow(Vector3Int coordinates)
	{
		return _terrainMap.GetTerrainHeightBelow(coordinates);
	}

	public IEnumerable<Vector3Int> GetAllHeightsInCell(Vector2Int cellCoordinates)
	{
		for (int z = 0; z < _mapSize.TerrainSize.z; z++)
		{
			Vector3Int vector3Int = cellCoordinates.ToVector3Int(z);
			if (!_terrainMap.IsTerrainVoxel(vector3Int) && _terrainMap.IsTerrainVoxel(cellCoordinates.ToVector3Int(z - 1)))
			{
				yield return vector3Int;
			}
		}
	}

	public bool UnsafeCellIsTerrain(int index)
	{
		return _terrainMap.UnsafeIsTerrainVoxel(index);
	}

	public bool CellIsField(Vector3Int cellCoordinates)
	{
		return _fieldMap.Get(cellCoordinates);
	}

	public bool CellIsCutout(Vector3Int cellCoordinates)
	{
		return _cutoutMap.Get(cellCoordinates) > 0;
	}

	public bool Underground(Vector3Int coords)
	{
		return _terrainMap.IsTerrainVoxel(coords);
	}

	public bool OnGround(Vector3Int coords)
	{
		if (_terrainMap.IsTerrainVoxel(coords.Below()))
		{
			return !_terrainMap.IsTerrainVoxel(coords);
		}
		return false;
	}

	public void SetTerrain(Vector3Int coordinates, int heightChange = 1)
	{
		Vector2Int vector2Int = coordinates.XY();
		if (coordinates.z + heightChange >= Size.z)
		{
			heightChange = Size.z - 1 - coordinates.z;
		}
		while (_terrainMap.IsTerrainVoxel(coordinates))
		{
			coordinates.z++;
			heightChange--;
		}
		if (!Contains(vector2Int) || heightChange <= 0)
		{
			return;
		}
		PrepareChangeData(vector2Int, coordinates.z, heightChange, setTerrain: true);
		foreach (TerrainHeightChange terrainHeightChange in _terrainHeightChanges)
		{
			PreTerrainHeightChanged?.Invoke(this, new TerrainHeightChangeEventArgs(terrainHeightChange));
			for (int i = terrainHeightChange.From; i <= terrainHeightChange.To; i++)
			{
				_terrainMap.SetTerrain(terrainHeightChange.Coordinates.ToVector3Int(i));
			}
			if (terrainHeightChange.To > MaxTerrainHeight)
			{
				MaxTerrainHeight = terrainHeightChange.To;
				MinMaxTerrainHeightChanged?.Invoke(this, EventArgs.Empty);
			}
			else if (terrainHeightChange.From == MinTerrainHeight)
			{
				CalculateMinAndMaxHeight();
			}
			TerrainHeightChanged?.Invoke(this, new TerrainHeightChangeEventArgs(terrainHeightChange));
		}
		_terrainHeightChanges.Clear();
	}

	public void UnsetTerrain(Vector3Int coordinates, int heightChange = 1)
	{
		Vector2Int vector2Int = coordinates.XY();
		int num = coordinates.z + 1;
		if (num - heightChange < 0)
		{
			heightChange += num - heightChange;
		}
		while (!_terrainMap.IsTerrainVoxel(coordinates))
		{
			coordinates.z--;
			heightChange--;
		}
		if (!Contains(vector2Int) || heightChange <= 0)
		{
			return;
		}
		int destroyedLayersCount = GetDestroyedLayersCount(coordinates, heightChange);
		PrepareChangeData(vector2Int, coordinates.z - destroyedLayersCount + 1, destroyedLayersCount, setTerrain: false);
		foreach (TerrainHeightChange terrainHeightChange in _terrainHeightChanges)
		{
			PreTerrainHeightChanged?.Invoke(this, new TerrainHeightChangeEventArgs(terrainHeightChange));
			for (int i = terrainHeightChange.From; i <= terrainHeightChange.To; i++)
			{
				_terrainMap.UnsetTerrain(terrainHeightChange.Coordinates.ToVector3Int(i));
			}
			if (terrainHeightChange.From < MinTerrainHeight)
			{
				MinTerrainHeight = terrainHeightChange.From;
				MinMaxTerrainHeightChanged?.Invoke(this, EventArgs.Empty);
			}
			else if (terrainHeightChange.To + 1 == MaxTerrainHeight)
			{
				CalculateMinAndMaxHeight();
			}
			TerrainHeightChanged?.Invoke(this, new TerrainHeightChangeEventArgs(terrainHeightChange));
		}
		_terrainHeightChanges.Clear();
	}

	public void UnsetTerrain(HashSet<Vector3Int> coordinatesToDestroy)
	{
		foreach (Vector3Int item in coordinatesToDestroy)
		{
			Vector2Int key = item.XY();
			if (!_tileChanges.TryGetValue(key, out var value))
			{
				value = new List<int>();
				_tileChanges[key] = value;
			}
			value.Add(item.z);
		}
		foreach (KeyValuePair<Vector2Int, List<int>> tileChange in _tileChanges)
		{
			Vector2Int key2 = tileChange.Key;
			List<int> value2 = tileChange.Value;
			if (value2.IsEmpty())
			{
				continue;
			}
			value2.Sort();
			int num = value2[value2.Count - 1];
			int num2 = num;
			for (int num3 = value2.Count - 2; num3 >= 0; num3--)
			{
				int num4 = value2[num3];
				if (num4 != num2 - 1 || num3 == 0)
				{
					PrepareHeightChanges(key2.ToVector3Int(num), num - num2 + 1);
					num = num4;
				}
				num2 = num4;
			}
			if (num2 == num)
			{
				PrepareHeightChanges(key2.ToVector3Int(num), 1);
			}
			value2.Clear();
		}
		foreach (TerrainHeightChange terrainHeightChange in _terrainHeightChanges)
		{
			PreTerrainHeightChanged?.Invoke(this, new TerrainHeightChangeEventArgs(terrainHeightChange));
			for (int i = terrainHeightChange.From; i <= terrainHeightChange.To; i++)
			{
				_terrainMap.UnsetTerrain(terrainHeightChange.Coordinates.ToVector3Int(i));
			}
			TerrainHeightChanged?.Invoke(this, new TerrainHeightChangeEventArgs(terrainHeightChange));
		}
		CalculateMinAndMaxHeight();
		_terrainHeightChanges.Clear();
	}

	public void SetField(Vector3Int coordinates)
	{
		Set(coordinates, _fieldMap, value: true);
	}

	public void UnsetField(Vector3Int coordinates)
	{
		Set(coordinates, _fieldMap, value: false);
	}

	public void SetCutout(Vector3Int coordinates)
	{
		Set(coordinates, _cutoutMap, increase: true);
	}

	public void UnsetCutout(Vector3Int coordinates)
	{
		Set(coordinates, _cutoutMap, increase: false);
	}

	public bool Contains(Vector2Int coordinates)
	{
		return Sizing.SizeContains(Size, coordinates);
	}

	public bool Contains(Vector3Int coordinates)
	{
		return Sizing.SizeContains(Size, coordinates);
	}

	public Vector3Int Clamp(Vector3Int coordinates)
	{
		return new Vector3Int(Mathf.Clamp(coordinates.x, 0, Size.x - 1), Mathf.Clamp(coordinates.y, 0, Size.y - 1), Mathf.Clamp(coordinates.z, 0, Size.z));
	}

	public int GetColumnCount(int index)
	{
		return _columnTerrainMap.ColumnCount[index];
	}

	public int GetColumnFloor(int index3D)
	{
		return _columnTerrainMap.GetColumn(index3D).Floor;
	}

	public int GetColumnCeiling(int index3D)
	{
		return _columnTerrainMap.GetColumn(index3D).Ceiling;
	}

	public bool TryGetDistanceToTerrainAbove(Vector3Int coordinates, out int distance)
	{
		Vector2Int coords2D = coordinates.XY();
		for (int i = coordinates.z; i < Size.z; i++)
		{
			if (_terrainMap.IsTerrainVoxel(coords2D.ToVector3Int(i)))
			{
				distance = i - coordinates.z;
				return true;
			}
		}
		distance = -1;
		return false;
	}

	public bool IsVisible(Vector3Int coordinates)
	{
		Vector3Int[] neighbors6Vector3Int = Deltas.Neighbors6Vector3Int;
		foreach (Vector3Int vector3Int in neighbors6Vector3Int)
		{
			Vector3Int coordinates2 = coordinates + vector3Int;
			if (!_terrainMap.IsTerrainVoxel(coordinates2))
			{
				return true;
			}
		}
		return false;
	}

	private void PrepareHeightChanges(Vector3Int coordinates, int heightChange)
	{
		Vector2Int vector2Int = coordinates.XY();
		int num = coordinates.z + 1;
		if (num - heightChange < 0)
		{
			heightChange += num - heightChange;
		}
		while (!_terrainMap.IsTerrainVoxel(coordinates))
		{
			coordinates.z--;
			heightChange--;
		}
		if (Contains(vector2Int) && heightChange > 0)
		{
			int destroyedLayersCount = GetDestroyedLayersCount(coordinates, heightChange);
			PrepareChangeData(vector2Int, coordinates.z - destroyedLayersCount + 1, destroyedLayersCount, setTerrain: false);
		}
	}

	private void PrepareChangeData(Vector2Int columnCoordinates, int startZ, int height, bool setTerrain)
	{
		int num = -1;
		for (int i = 0; i < height; i++)
		{
			Vector3Int coordinates = columnCoordinates.ToVector3Int(startZ + i);
			if (num == -1 && setTerrain != _terrainMap.IsTerrainVoxel(coordinates))
			{
				num = startZ + i;
			}
			if (num != -1 && setTerrain == _terrainMap.IsTerrainVoxel(coordinates))
			{
				int to = startZ + i - 1;
				TerrainHeightChange item = new TerrainHeightChange(columnCoordinates, num, to, setTerrain);
				_terrainHeightChanges.Add(item);
				num = -1;
			}
		}
		if (num != -1)
		{
			TerrainHeightChange item2 = new TerrainHeightChange(columnCoordinates, num, startZ + height - 1, setTerrain);
			_terrainHeightChanges.Add(item2);
		}
	}

	private void Set(Vector3Int coordinates, TerrainPropertyMap<bool> map, bool value)
	{
		if (Contains(coordinates) && map.Get(coordinates) != value)
		{
			map.Set(coordinates, value);
			FieldOrCutoutChanged?.Invoke(this, coordinates);
		}
	}

	private void Set(Vector3Int coordinates, TerrainPropertyMap<int> map, bool increase)
	{
		if (Contains(coordinates))
		{
			int num = map.Get(coordinates);
			map.Set(coordinates, increase ? (num + 1) : Math.Max(0, num - 1));
			if (((num == 0) & increase) || (num == 1 && !increase))
			{
				FieldOrCutoutChanged?.Invoke(this, coordinates);
			}
		}
	}

	private void CalculateMinAndMaxHeight()
	{
		int minTerrainHeight = MinTerrainHeight;
		int maxTerrainHeight = MaxTerrainHeight;
		MinTerrainHeight = int.MaxValue;
		MaxTerrainHeight = 0;
		for (int i = 0; i < Size.y; i++)
		{
			for (int j = 0; j < Size.x; j++)
			{
				int num = _mapIndexService.CellToIndex(new Vector2Int(j, i));
				int columnCount = GetColumnCount(num);
				int ceiling = _columnTerrainMap.GetColumn(num).Ceiling;
				if (ceiling < MinTerrainHeight)
				{
					MinTerrainHeight = ceiling;
				}
				int index3D = num + (columnCount - 1) * _mapIndexService.VerticalStride;
				int ceiling2 = _columnTerrainMap.GetColumn(index3D).Ceiling;
				if (ceiling2 > MaxTerrainHeight)
				{
					MaxTerrainHeight = ceiling2;
				}
			}
		}
		if (minTerrainHeight == MinTerrainHeight || maxTerrainHeight != MaxTerrainHeight)
		{
			MinMaxTerrainHeightChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private int GetDestroyedLayersCount(Vector3Int coordinates, int heightChange)
	{
		int num = 0;
		for (int i = 0; i < heightChange; i++)
		{
			if (_terrainMap.IsTerrainVoxel(coordinates))
			{
				num++;
				coordinates = coordinates.Below();
				continue;
			}
			return num;
		}
		return num;
	}
}
