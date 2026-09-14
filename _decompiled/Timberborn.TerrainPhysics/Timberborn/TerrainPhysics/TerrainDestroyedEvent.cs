using UnityEngine;

namespace Timberborn.TerrainPhysics;

public class TerrainDestroyedEvent
{
	public Vector3Int Coordinates { get; }

	public TerrainDestroyedEvent(Vector3Int coordinates)
	{
		Coordinates = coordinates;
	}
}
