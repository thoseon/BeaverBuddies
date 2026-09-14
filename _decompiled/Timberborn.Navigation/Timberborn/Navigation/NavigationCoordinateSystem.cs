using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.Navigation;

public static class NavigationCoordinateSystem
{
	public static Vector3 GridToWorld(Vector3Int coordinates)
	{
		return CoordinateSystem.GridToWorldCentered(coordinates);
	}

	public static Vector3Int WorldToGridInt(Vector3 position)
	{
		Vector3 vector = new Vector3(0f, 0.1f, 0f);
		return CoordinateSystem.WorldToGridInt(position + vector);
	}
}
