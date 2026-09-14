using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timberborn.Common;

public static class VectorExtensions
{
	public static Vector2Int XY(this Vector3Int value)
	{
		return new Vector2Int(value.x, value.y);
	}

	public static Vector2 XY(this Vector3 value)
	{
		return new Vector2(value.x, value.y);
	}

	public static IEnumerable<Vector2Int> XY(this IEnumerable<Vector3Int> vectors)
	{
		return vectors.Select(XY);
	}

	public static Vector3Int XYZ(this Vector2Int value)
	{
		return new Vector3Int(value.x, value.y, 0);
	}

	public static Vector3 XYZ(this Vector2 value)
	{
		return new Vector3(value.x, value.y, 0f);
	}

	public static Vector2 XZ(this Vector3 value)
	{
		return new Vector2(value.x, value.z);
	}

	public static Vector3Int Above(this Vector3Int value)
	{
		return new Vector3Int(value.x, value.y, value.z + 1);
	}

	public static Vector3Int Below(this Vector3Int value)
	{
		return new Vector3Int(value.x, value.y, value.z - 1);
	}

	public static Vector3Int FloorToInt(this Vector3 value)
	{
		return new Vector3Int(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y), Mathf.FloorToInt(value.z));
	}

	public static Vector3Int CeilToInt(this Vector3 value)
	{
		return new Vector3Int(Mathf.CeilToInt(value.x), Mathf.CeilToInt(value.y), Mathf.CeilToInt(value.z));
	}

	public static Vector2Int FloorToInt(this Vector2 value)
	{
		return new Vector2Int(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y));
	}

	public static Vector3Int ToVector3Int(this Vector2Int coords2D, int z)
	{
		return new Vector3Int(coords2D.x, coords2D.y, z);
	}

	public static Vector3 ToVector3(this Vector2 coords2D, int z)
	{
		return new Vector3(coords2D.x, coords2D.y, z);
	}
}
