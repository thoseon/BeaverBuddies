using UnityEngine;

namespace Timberborn.Common;

public static class Sizing
{
	public static bool SizeContains(Vector2Int size, Vector2Int coordinates)
	{
		if (coordinates.x >= 0 && coordinates.x < size.x && coordinates.y >= 0)
		{
			return coordinates.y < size.y;
		}
		return false;
	}

	public static bool SizeContains(Vector3Int size, Vector2Int coordinates)
	{
		if (coordinates.x >= 0 && coordinates.x < size.x && coordinates.y >= 0)
		{
			return coordinates.y < size.y;
		}
		return false;
	}

	public static bool SizeContains(Vector3Int size, Vector3Int coordinates)
	{
		if (coordinates.x >= 0 && coordinates.x < size.x && coordinates.y >= 0 && coordinates.y < size.y && coordinates.z >= 0)
		{
			return coordinates.z < size.z;
		}
		return false;
	}
}
