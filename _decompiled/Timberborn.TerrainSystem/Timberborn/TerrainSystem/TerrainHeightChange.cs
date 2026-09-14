using UnityEngine;

namespace Timberborn.TerrainSystem;

public readonly struct TerrainHeightChange
{
	public Vector2Int Coordinates { get; }

	public int From { get; }

	public int To { get; }

	public bool SetTerrain { get; }

	public TerrainHeightChange(Vector2Int coordinates, int from, int to, bool setTerrain)
	{
		Coordinates = coordinates;
		From = from;
		To = to;
		SetTerrain = setTerrain;
	}
}
