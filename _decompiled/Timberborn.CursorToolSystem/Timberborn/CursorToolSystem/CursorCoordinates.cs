using UnityEngine;

namespace Timberborn.CursorToolSystem;

public readonly struct CursorCoordinates
{
	public Vector3 Coordinates { get; }

	public Vector3Int TileCoordinates { get; }

	public CursorCoordinates(Vector3 coordinates, Vector3Int tileCoordinates)
	{
		Coordinates = coordinates;
		TileCoordinates = tileCoordinates;
	}
}
