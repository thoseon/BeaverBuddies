using System;
using UnityEngine;

namespace Timberborn.Common;

public class Array3D<TValue>
{
	private readonly TValue[,,] _values;

	private readonly Vector3Int _size;

	private readonly TValue _defaultValue;

	public Array3D(Vector3Int size, Func<TValue> defaultValueProvider)
	{
		_values = new TValue[size.x, size.y, size.z];
		_size = size;
		_defaultValue = defaultValueProvider();
		for (int i = 0; i < size.x; i++)
		{
			for (int j = 0; j < size.y; j++)
			{
				for (int k = 0; k < size.z; k++)
				{
					GetRefAtWithoutBoundsCheck(new Vector3Int(i, j, k)) = defaultValueProvider();
				}
			}
		}
	}

	public TValue GetCopyAtOrDefault(Vector3Int coordinates)
	{
		if (!Contains(coordinates))
		{
			return _defaultValue;
		}
		return GetRefAtWithoutBoundsCheck(coordinates);
	}

	public ref TValue GetRefAt(Vector3Int coordinates)
	{
		if (!Contains(coordinates))
		{
			throw new ArgumentException($"{coordinates} is out of bounds");
		}
		return ref GetRefAtWithoutBoundsCheck(coordinates);
	}

	public bool Contains(Vector3Int coordinates)
	{
		return Sizing.SizeContains(_size, coordinates);
	}

	public bool Contains(Vector2Int coordinates)
	{
		return Sizing.SizeContains(_size, coordinates);
	}

	public void Clear()
	{
		Array.Clear(_values, 0, _values.Length);
	}

	private ref TValue GetRefAtWithoutBoundsCheck(Vector3Int coordinates)
	{
		return ref _values[coordinates.x, coordinates.y, coordinates.z];
	}
}
