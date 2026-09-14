using System;
using System.Runtime.InteropServices;
using Timberborn.Common;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.TerrainSystem;

internal class ColumnTerrainMap : ILoadableSingleton
{
	private readonly MapIndexService _mapIndexService;

	private readonly TerrainMap _terrainMap;

	private TerrainColumn[] _terrainColumns;

	private int _verticalStride;

	public byte[] ColumnCount { get; private set; }

	public bool AnyColumnChanged { get; private set; }

	public int MaxColumnCount { get; private set; } = 1;

	public event EventHandler<ColumnAddedEventArgs> ColumnAdded;

	public event EventHandler<ColumnRemovedEventArgs> ColumnRemoved;

	public ColumnTerrainMap(MapIndexService mapIndexService, TerrainMap terrainMap)
	{
		_mapIndexService = mapIndexService;
		_terrainMap = terrainMap;
	}

	public void Load()
	{
		_verticalStride = _mapIndexService.VerticalStride;
		_terrainColumns = new TerrainColumn[_verticalStride];
		ColumnCount = new byte[_verticalStride];
		LoadColumns();
		_terrainMap.TerrainAdded += OnTerrainAdded;
		_terrainMap.TerrainRemoved += OnTerrainRemoved;
	}

	public ref TerrainColumn GetColumn(int index3D)
	{
		return ref _terrainColumns[index3D];
	}

	public void CopyTerrainColumnsData(ReadOnlyTerrainColumn[] terrainColumns, byte[] columnCount, int levels)
	{
		int length = levels * _verticalStride;
		MemoryMarshal.Cast<TerrainColumn, ReadOnlyTerrainColumn>(MemoryExtensions.AsSpan(_terrainColumns, 0, length)).CopyTo(terrainColumns);
		ColumnCount.CopyTo(columnCount, 0);
		AnyColumnChanged = false;
	}

	private void OnTerrainAdded(object sender, Vector3Int coords)
	{
		OnTerrainChanged(coords, added: true);
	}

	private void OnTerrainRemoved(object sender, Vector3Int coords)
	{
		OnTerrainChanged(coords, added: false);
	}

	private void OnTerrainChanged(Vector3Int coords, bool added)
	{
		if (added)
		{
			AddTerrain(coords);
		}
		else
		{
			RemoveTerrain(coords);
		}
	}

	private void AddTerrain(Vector3Int coordinates)
	{
		int num = _mapIndexService.CellToIndex(coordinates.XY());
		int z = coordinates.z;
		int columnIndex = GetColumnIndex(num, z + 1);
		int num2 = ((z != 0) ? GetColumnIndex(num, z - 1) : 0);
		if (columnIndex == -1 && num2 == -1)
		{
			InsertNewColumn(z, num);
		}
		else if (columnIndex != -1 && num2 != -1)
		{
			MergeColumns(num, columnIndex, num2);
		}
		else if (columnIndex != -1)
		{
			_terrainColumns[columnIndex * _verticalStride + num].Floor = z;
		}
		else if (num2 != -1)
		{
			_terrainColumns[num2 * _verticalStride + num].Ceiling = z + 1;
		}
		else
		{
			InsertNewColumn(z, num);
		}
		AnyColumnChanged = true;
	}

	private void RemoveTerrain(Vector3Int coordinates)
	{
		int index = _mapIndexService.CellToIndex(coordinates.XY());
		int z = coordinates.z;
		ref TerrainColumn column = ref GetColumn(index, z);
		if (column.Floor == z)
		{
			if (column.Ceiling - 1 == z)
			{
				if (z != 0)
				{
					RemoveColumn(index, GetColumnIndex(index, z));
				}
				else
				{
					column.Ceiling = 0;
				}
			}
			else if (z == 0)
			{
				SplitColumn(ref column, index, 0, 1);
			}
			else
			{
				column.Floor++;
			}
		}
		else if (column.Ceiling - 1 == z)
		{
			column.Ceiling--;
		}
		else
		{
			SplitColumn(ref column, index, z, z + 1);
		}
		AnyColumnChanged = true;
	}

	private int GetColumnIndex(int index, int height)
	{
		byte b = ColumnCount[index];
		for (int i = 0; i < b; i++)
		{
			ref TerrainColumn reference = ref _terrainColumns[i * _verticalStride + index];
			if (height >= reference.Floor && height < reference.Ceiling)
			{
				return i;
			}
		}
		return -1;
	}

	private ref TerrainColumn GetColumn(int index, int height)
	{
		byte b = ColumnCount[index];
		for (int i = 0; i < b; i++)
		{
			ref TerrainColumn reference = ref _terrainColumns[i * _verticalStride + index];
			if (height < reference.Floor)
			{
				break;
			}
			if (height < reference.Ceiling)
			{
				return ref reference;
			}
		}
		throw new InvalidOperationException($"Column for index {index} and height {height} not found");
	}

	private void InsertNewColumn(int height, int index)
	{
		TerrainColumn newColumn = new TerrainColumn(height, height + 1);
		InsertColumn(index, height, ref newColumn);
	}

	private void MergeColumns(int index, int columnAboveIndex, int columnBelowIndex)
	{
		ref TerrainColumn reference = ref _terrainColumns[columnAboveIndex * _verticalStride + index];
		_terrainColumns[columnBelowIndex * _verticalStride + index].Ceiling = reference.Ceiling;
		RemoveColumn(index, columnAboveIndex);
	}

	private void RemoveColumn(int index, int columnIndex)
	{
		byte b = ColumnCount[index];
		for (int i = columnIndex + 1; i < b; i++)
		{
			int num = i * _verticalStride + index;
			int num2 = (i - 1) * _verticalStride + index;
			_terrainColumns[num2] = _terrainColumns[num];
		}
		ColumnCount[index]--;
		int num3 = (b - 1) * _verticalStride + index;
		_terrainColumns[num3] = default(TerrainColumn);
		ColumnRemoved?.Invoke(this, new ColumnRemovedEventArgs(columnIndex * _verticalStride + index, ColumnCount[index]));
	}

	private void SplitColumn(ref TerrainColumn column, int index, int splitHeight, int newFloorHeight)
	{
		TerrainColumn newColumn = new TerrainColumn(newFloorHeight, column.Ceiling);
		column.Ceiling = splitHeight;
		InsertColumn(index, splitHeight, ref newColumn);
	}

	private void InsertColumn(int index, int splitHeight, ref TerrainColumn newColumn)
	{
		byte b = ColumnCount[index];
		for (int num = ColumnCount[index] - 1; num >= 0; num--)
		{
			int num2 = num * _verticalStride + index;
			ref TerrainColumn reference = ref _terrainColumns[num2];
			if (reference.Floor <= splitHeight)
			{
				break;
			}
			int num3 = num + 1;
			if (num3 == MaxColumnCount)
			{
				IncreaseMaxColumnCount();
			}
			int num4 = num3 * _verticalStride + index;
			_terrainColumns[num4] = reference;
			b = (byte)num;
		}
		int num5 = b * _verticalStride + index;
		if (ColumnCount[index]++ == MaxColumnCount)
		{
			IncreaseMaxColumnCount();
		}
		_terrainColumns[num5] = newColumn;
		ColumnAdded?.Invoke(this, new ColumnAddedEventArgs(num5, ColumnCount[index]));
	}

	private void IncreaseMaxColumnCount()
	{
		MaxColumnCount++;
		Resize(MaxColumnCount);
	}

	private void Resize(int currentMaxIndex)
	{
		int newSize = currentMaxIndex * _mapIndexService.VerticalStride;
		Array.Resize(ref _terrainColumns, newSize);
	}

	private void LoadColumns()
	{
		Vector3Int terrainSize = _mapIndexService.TerrainSize;
		foreach (int item in _mapIndexService.Indices2D)
		{
			int num = -1;
			bool flag = true;
			int floor = 0;
			for (int i = 0; i < terrainSize.z; i++)
			{
				int index = item + i * _verticalStride;
				if (_terrainMap.UnsafeIsTerrainVoxel(index))
				{
					if (!flag)
					{
						floor = i;
					}
					flag = true;
				}
				else
				{
					if (flag)
					{
						CreateColumn(++num, item, new TerrainColumn(floor, i));
					}
					flag = false;
				}
			}
			if (flag)
			{
				CreateColumn(++num, item, new TerrainColumn(floor, terrainSize.z));
			}
			ColumnCount[item] = (byte)(num + 1);
		}
		AnyColumnChanged = true;
	}

	private void CreateColumn(int columnIndex, int index2D, TerrainColumn column)
	{
		if (columnIndex + 1 > MaxColumnCount)
		{
			IncreaseMaxColumnCount();
		}
		_terrainColumns[index2D + columnIndex * _verticalStride] = column;
	}
}
