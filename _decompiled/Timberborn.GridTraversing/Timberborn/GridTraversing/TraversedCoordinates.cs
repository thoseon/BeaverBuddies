using UnityEngine;

namespace Timberborn.GridTraversing;

public readonly struct TraversedCoordinates
{
	public Vector3Int Coordinates { get; }

	public Vector3Int Face { get; }

	public Vector3 Intersection { get; }

	public Vector3Int CoordinatesWithFaceOffset { get; }

	public TraversedCoordinates(Vector3Int coordinates, Vector3Int face, Vector3 intersection)
	{
		Coordinates = coordinates;
		Face = face;
		Intersection = intersection;
		CoordinatesWithFaceOffset = coordinates + face;
	}
}
