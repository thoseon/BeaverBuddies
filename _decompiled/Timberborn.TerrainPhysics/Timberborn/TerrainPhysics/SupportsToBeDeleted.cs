using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.TerrainPhysics;

internal class SupportsToBeDeleted
{
	private readonly HashSet<Vector3Int> _supportsToBeDeleted = new HashSet<Vector3Int>();

	public void Mark(Vector3Int coordinates)
	{
		_supportsToBeDeleted.Add(coordinates);
	}

	public bool IsMarked(Vector3Int coordinates)
	{
		return _supportsToBeDeleted.Contains(coordinates);
	}

	public void Clear()
	{
		_supportsToBeDeleted.Clear();
	}
}
