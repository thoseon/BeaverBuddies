using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Planting;

public class PlantingMap
{
	private Vector3Int _size;

	private readonly string[,,] _resourceIds;

	public PlantingMap(Vector3Int size)
	{
		_size = size;
		_resourceIds = new string[size.x, size.y, size.z];
	}

	public IEnumerable<Vector3Int> GetCoordinatesWithSetResource()
	{
		for (int x = 0; x < _size.x; x++)
		{
			for (int y = 0; y < _size.y; y++)
			{
				for (int z = 0; z < _size.z; z++)
				{
					if (_resourceIds[x, y, z] != null)
					{
						yield return new Vector3Int(x, y, z);
					}
				}
			}
		}
	}

	public string GetResource(Vector3Int coordinates)
	{
		if (!Contains(coordinates))
		{
			return null;
		}
		return _resourceIds[coordinates.x, coordinates.y, coordinates.z];
	}

	public void SetResource(IEnumerable<Vector3Int> coordinates, string resource)
	{
		foreach (Vector3Int coordinate in coordinates)
		{
			SetResource(coordinate, resource);
		}
	}

	public void SetResourceIfEmpty(IEnumerable<Vector3Int> coordinates, string resource)
	{
		foreach (Vector3Int coordinate in coordinates)
		{
			if (GetResource(coordinate) == null)
			{
				SetResource(coordinate, resource);
			}
		}
	}

	public void SetResource(Vector3Int coordinates, string resource)
	{
		if (Contains(coordinates))
		{
			_resourceIds[coordinates.x, coordinates.y, coordinates.z] = resource;
		}
	}

	public void UnsetResource(Vector3Int coordinates)
	{
		if (Contains(coordinates))
		{
			_resourceIds[coordinates.x, coordinates.y, coordinates.z] = null;
		}
	}

	private bool Contains(Vector3Int coordinates)
	{
		return Sizing.SizeContains(_size, coordinates);
	}
}
