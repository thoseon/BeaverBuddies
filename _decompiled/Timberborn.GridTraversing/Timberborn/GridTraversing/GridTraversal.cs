using System;
using System.Collections.Generic;
using Timberborn.MapStateSystem;
using UnityEngine;

namespace Timberborn.GridTraversing;

public class GridTraversal
{
	private readonly MapSize _mapSize;

	public GridTraversal(MapSize mapSize)
	{
		_mapSize = mapSize;
	}

	public IEnumerable<TraversedCoordinates> TraverseRay(Ray ray)
	{
		int levelAboveMaxBuildingLevel = _mapSize.TotalSize.z + 2;
		Vector3 origin = ray.origin;
		Vector3 direction = ray.direction.normalized;
		if (origin.z > (float)levelAboveMaxBuildingLevel && new Plane(new Vector3(0f, 0f, -1f), levelAboveMaxBuildingLevel).Raycast(ray, out var enter))
		{
			origin = ray.GetPoint(enter);
		}
		if (origin.z < -1f && new Plane(new Vector3(0f, 0f, -1f), -1f).Raycast(ray, out var enter2))
		{
			origin = ray.GetPoint(enter2);
		}
		int x = Mathf.FloorToInt(origin.x);
		int y = Mathf.FloorToInt(origin.y);
		int z = Mathf.FloorToInt(origin.z);
		int stepX = Math.Sign(direction.x);
		int stepY = Math.Sign(direction.y);
		int stepZ = Math.Sign(direction.z);
		double tMaxX = Intbound(origin.x, direction.x);
		double tMaxY = Intbound(origin.y, direction.y);
		double tMaxZ = Intbound(origin.z, direction.z);
		double tDeltaX = ((direction.x == 0f) ? double.PositiveInfinity : ((double)((float)stepX / direction.x)));
		double tDeltaY = ((direction.y == 0f) ? double.PositiveInfinity : ((double)((float)stepY / direction.y)));
		double tDeltaZ = ((direction.z == 0f) ? double.PositiveInfinity : ((double)((float)stepZ / direction.z)));
		int iteration = 0;
		while (iteration < 10000)
		{
			double num;
			Vector3Int face;
			if (tMaxX < tMaxZ)
			{
				if (tMaxX < tMaxY)
				{
					num = tMaxX;
					x += stepX;
					tMaxX += tDeltaX;
					face = new Vector3Int(-stepX, 0, 0);
				}
				else
				{
					num = tMaxY;
					y += stepY;
					tMaxY += tDeltaY;
					face = new Vector3Int(0, -stepY, 0);
				}
			}
			else if (tMaxZ < tMaxY)
			{
				num = tMaxZ;
				z += stepZ;
				tMaxZ += tDeltaZ;
				face = new Vector3Int(0, 0, -stepZ);
			}
			else
			{
				num = tMaxY;
				y += stepY;
				tMaxY += tDeltaY;
				face = new Vector3Int(0, -stepY, 0);
			}
			if (z < -1 || z > levelAboveMaxBuildingLevel)
			{
				break;
			}
			Vector3Int coordinates = new Vector3Int(x, y, z);
			Vector3 intersection = origin + direction * (float)num;
			yield return new TraversedCoordinates(coordinates, face, intersection);
			int num2 = iteration + 1;
			iteration = num2;
		}
	}

	private static double Intbound(double s, double ds)
	{
		if (ds == 0.0)
		{
			return double.PositiveInfinity;
		}
		return ((ds > 0.0) ? (Math.Ceiling(s) - s) : (s - Math.Floor(s))) / Math.Abs(ds);
	}
}
