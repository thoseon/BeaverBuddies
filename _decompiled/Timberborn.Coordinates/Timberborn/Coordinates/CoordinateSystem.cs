using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Coordinates;

public static class CoordinateSystem
{
	public static Vector3 WorldToGrid(Vector3 position)
	{
		return new Vector3(position.x, position.z, position.y);
	}

	public static Vector3Int WorldToGridInt(Vector3 position)
	{
		return WorldToGrid(position).FloorToInt();
	}

	public static Ray WorldToGrid(Ray ray)
	{
		return new Ray(WorldToGrid(ray.origin), WorldToGrid(ray.direction));
	}

	public static Vector3 GridToWorld(Vector3Int coordinates)
	{
		return new Vector3(coordinates.x, coordinates.z, coordinates.y);
	}

	public static Vector3 GridToWorld(Vector3 coordinates)
	{
		return new Vector3(coordinates.x, coordinates.z, coordinates.y);
	}

	public static Vector3 GridToWorldCentered(Vector3Int coordinates)
	{
		return CenterWorld(GridToWorld(coordinates));
	}

	public static Vector3 GridToWorldCentered(Vector3 coordinates)
	{
		return CenterWorld(GridToWorld(coordinates));
	}

	public static Ray GridToWorld(Ray ray)
	{
		return new Ray(GridToWorld(ray.origin), GridToWorld(ray.direction));
	}

	private static Vector3 CenterWorld(Vector3 position)
	{
		return position + new Vector3(0.5f, 0f, 0.5f);
	}
}
