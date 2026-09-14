using System;
using UnityEngine;

namespace Timberborn.Common;

public static class Vectors
{
	public static (Vector3Int min, Vector3Int max) MinMax(Vector3Int a, Vector3Int b)
	{
		return (min: Min(a, b), max: Max(a, b));
	}

	public static (Vector2Int min, Vector2Int max) MinMax(Vector2Int a, Vector2Int b)
	{
		return (min: Min(a, b), max: Max(a, b));
	}

	private static Vector3Int Min(Vector3Int a, Vector3Int b)
	{
		int x = Math.Min(a.x, b.x);
		int y = Math.Min(a.y, b.y);
		int z = Math.Min(a.z, b.z);
		return new Vector3Int(x, y, z);
	}

	private static Vector3Int Max(Vector3Int a, Vector3Int b)
	{
		int x = Math.Max(a.x, b.x);
		int y = Math.Max(a.y, b.y);
		int z = Math.Max(a.z, b.z);
		return new Vector3Int(x, y, z);
	}

	private static Vector2Int Min(Vector2Int a, Vector2Int b)
	{
		int x = Math.Min(a.x, b.x);
		int y = Math.Min(a.y, b.y);
		return new Vector2Int(x, y);
	}

	private static Vector2Int Max(Vector2Int a, Vector2Int b)
	{
		int x = Math.Max(a.x, b.x);
		int y = Math.Max(a.y, b.y);
		return new Vector2Int(x, y);
	}
}
