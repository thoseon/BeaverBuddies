using UnityEngine;

namespace Timberborn.BlockObjectPickingSystem;

public readonly struct PickedCoordinates
{
	public Vector3Int Coordinates { get; }

	public float ReferenceTerrainLevel { get; }

	public int VerticalOffset { get; }

	public bool FilterOverhangingCoordinates { get; }

	public PickedCoordinates(Vector3Int coordinates, float referenceTerrainLevel, int verticalOffset, bool filterOverhangingCoordinates)
	{
		Coordinates = coordinates;
		ReferenceTerrainLevel = referenceTerrainLevel;
		VerticalOffset = verticalOffset;
		FilterOverhangingCoordinates = filterOverhangingCoordinates;
	}
}
