using Timberborn.Common;
using UnityEngine;

namespace Timberborn.TerrainSystem;

public class TerrainPropertyMap<T>
{
	private readonly T[,,] _cells;

	private readonly Vector3Int _size;

	public TerrainPropertyMap(Vector3Int size)
	{
		_size = size;
		_cells = new T[size.y, size.x, size.z];
	}

	public T Get(Vector3Int coordinates)
	{
		if (!Contains(coordinates))
		{
			return default(T);
		}
		return _cells[coordinates.y, coordinates.x, coordinates.z];
	}

	public void Set(Vector3Int coordinates, T value)
	{
		if (Contains(coordinates))
		{
			_cells[coordinates.y, coordinates.x, coordinates.z] = value;
		}
	}

	private bool Contains(Vector3Int coordinates)
	{
		return Sizing.SizeContains(_size, coordinates);
	}
}
