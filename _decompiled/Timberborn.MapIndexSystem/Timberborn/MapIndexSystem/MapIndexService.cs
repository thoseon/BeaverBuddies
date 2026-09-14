using System;
using Timberborn.MapStateSystem;
using Timberborn.PackedListSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.MapIndexSystem;

public class MapIndexService : ILoadableSingleton
{
	private static readonly int Margin = 1;

	private readonly MapSize _mapSize;

	private bool[] _actualMap;

	private int _startingIndex;

	public int Stride { get; private set; }

	public int VerticalStride { get; private set; }

	public int MaxIndex { get; private set; }

	public int MaxSize3D { get; private set; }

	public Vector3Int TerrainSize => _mapSize.TerrainSize;

	public Vector3Int TotalSize => _mapSize.TotalSize;

	public Index2DEnumerator Indices2D => new Index2DEnumerator(TerrainSize.x, TerrainSize.y, Margin, _startingIndex);

	public MapIndexService(MapSize mapSize)
	{
		_mapSize = mapSize;
	}

	public void Load()
	{
		int num = Margin * 2;
		int num2 = TerrainSize.x + num;
		int num3 = TerrainSize.y + num;
		Stride = num2;
		VerticalStride = num2 * num3;
		_startingIndex = Stride * Margin + Margin;
		MaxIndex = num2 * num3;
		MaxSize3D = VerticalStride * TerrainSize.z;
		InitializeActualMap();
	}

	public int CellToIndex(Vector2Int coordinates)
	{
		return (coordinates.y + 1) * Stride + coordinates.x + 1;
	}

	public int CoordinatesToIndex3D(Vector3Int coordinates)
	{
		return (coordinates.y + 1) * Stride + coordinates.x + 1 + coordinates.z * VerticalStride;
	}

	public Vector3Int Index3DToCoordinates(int index3D)
	{
		int num = index3D / VerticalStride;
		int num2 = index3D - num * VerticalStride;
		int num3 = num2 % Stride;
		int num4 = num2 / Stride;
		return new Vector3Int(num3 - 1, num4 - 1, num);
	}

	public Vector3Int IndexToCoordinates(int index, int height)
	{
		int num = index % Stride;
		int num2 = index / Stride;
		return new Vector3Int(num - 1, num2 - 1, height);
	}

	public int Index2DTo3D(int index, int z)
	{
		int num = index % Stride;
		return index / Stride * Stride + num + z * VerticalStride;
	}

	public int CoordinatesToActualMapIndex(Vector2Int coordinates)
	{
		return coordinates.x + coordinates.y * TerrainSize.x;
	}

	public bool IndexIsInActualMap(int index)
	{
		return _actualMap[index];
	}

	public PackedList<T> Pack<T>(T[] inputArray, int levels = 1)
	{
		return Pack(MemoryExtensions.AsSpan(inputArray), levels);
	}

	public PackedList<T> Pack<T>(ReadOnlySpan<T> inputArray, int levels = 1)
	{
		Vector3Int terrainSize = _mapSize.TerrainSize;
		int num = _startingIndex;
		int num2 = 0;
		int num3 = terrainSize.x * terrainSize.y;
		T[] array = new T[num3 * levels];
		for (int i = 0; i < terrainSize.y; i++)
		{
			for (int j = 0; j < terrainSize.x; j++)
			{
				for (int k = 0; k < levels; k++)
				{
					int num4 = num2 + k * num3;
					int index = num + k * VerticalStride;
					array[num4] = inputArray[index];
				}
				num++;
				num2++;
			}
			num += 2;
		}
		return new PackedList<T>(array);
	}

	public PackedList<T> Pack3D<T>(T[] inputArray)
	{
		return Pack(inputArray, _mapSize.TerrainSize.z);
	}

	public T[] Unpack2DHeightData<T>(PackedList<T> packedList, int levels = 1)
	{
		T[] array = packedList.Array;
		Vector3Int terrainSize = _mapSize.TerrainSize;
		int num = 0;
		int num2 = terrainSize.x * terrainSize.y;
		int num3 = 0;
		T[] array2 = new T[array.Length];
		for (int i = 0; i < terrainSize.y; i++)
		{
			for (int j = 0; j < terrainSize.x; j++)
			{
				for (int k = 0; k < levels; k++)
				{
					int num4 = num3 + k * VerticalStride;
					int num5 = num + k * num2;
					array2[num4] = array[num5];
				}
				num++;
				num3++;
			}
		}
		return array2;
	}

	public T[] Unpack3D<T>(T[] inputArray)
	{
		Vector3Int terrainSize = _mapSize.TerrainSize;
		int z = _mapSize.TerrainSize.z;
		int num = 0;
		int num2 = terrainSize.x * terrainSize.y;
		int num3 = _startingIndex;
		T[] array = new T[MaxSize3D];
		for (int i = 0; i < terrainSize.y; i++)
		{
			for (int j = 0; j < terrainSize.x; j++)
			{
				for (int k = 0; k < z; k++)
				{
					int num4 = num3 + k * VerticalStride;
					int num5 = num + k * num2;
					array[num4] = inputArray[num5];
				}
				num++;
				num3++;
			}
			num3 += 2;
		}
		return array;
	}

	public T[] Unpack<T>(PackedList<T> packedList, int levels = 1)
	{
		T[] array = packedList.Array;
		Vector3Int terrainSize = _mapSize.TerrainSize;
		int num = 0;
		int num2 = terrainSize.x * terrainSize.y;
		int num3 = _startingIndex;
		T[] array2 = new T[MaxIndex * levels];
		for (int i = 0; i < terrainSize.y; i++)
		{
			for (int j = 0; j < terrainSize.x; j++)
			{
				for (int k = 0; k < levels; k++)
				{
					int num4 = num3 + k * VerticalStride;
					int num5 = num + k * num2;
					array2[num4] = array[num5];
				}
				num++;
				num3++;
			}
			num3 += 2;
		}
		return array2;
	}

	public void Unpack<T>(PackedList<T> packedList, Span<T> unpacked, int levels = 1)
	{
		T[] array = packedList.Array;
		Vector3Int terrainSize = _mapSize.TerrainSize;
		int num = 0;
		int num2 = terrainSize.x * terrainSize.y;
		int num3 = _startingIndex;
		for (int i = 0; i < terrainSize.y; i++)
		{
			for (int j = 0; j < terrainSize.x; j++)
			{
				for (int k = 0; k < levels; k++)
				{
					int num4 = num3 + k * VerticalStride;
					int num5 = num + k * num2;
					if (num4 < unpacked.Length)
					{
						unpacked[num4] = array[num5];
					}
				}
				num++;
				num3++;
			}
			num3 += 2;
		}
	}

	private void InitializeActualMap()
	{
		_actualMap = new bool[MaxIndex];
		int num = _startingIndex;
		for (int i = 0; i < TerrainSize.y; i++)
		{
			for (int j = 0; j < TerrainSize.x; j++)
			{
				_actualMap[num] = true;
				num++;
			}
			num += 2;
		}
	}
}
