using UnityEngine;

namespace Timberborn.ConstructionGuidelines;

public readonly struct FootprintCoordinates
{
	public bool CanHaveFootprint { get; }

	public Vector3Int Coordinates { get; }

	public FootprintCoordinates(Vector3Int coordinates, bool canHaveFootprint)
	{
		Coordinates = coordinates;
		CanHaveFootprint = canHaveFootprint;
	}
}
