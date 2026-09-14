using System;
using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class AreaIterator
{
	private readonly AreaClamper _areaClamper;

	public AreaIterator(AreaClamper areaClamper)
	{
		_areaClamper = areaClamper;
	}

	public IEnumerable<Vector3Int> GetRectangle(Vector3Int start, Vector3Int end, int maxBlocks)
	{
		int maxSize = (int)Math.Sqrt(maxBlocks);
		return GetCuboid(end: new Vector3Int(end.x, end.y, start.z), start: start, maxSize: maxSize);
	}

	public IEnumerable<Vector3Int> GetLine(Vector3Int start, Vector3Int end, int maxPoints, out LineDirection direction)
	{
		if (start == end)
		{
			direction = LineDirection.SinglePoint;
			return Enumerables.One(start);
		}
		Vector3Int furthestLineEnd = GetFurthestLineEnd(start, end, out direction);
		return GetCuboid(start, furthestLineEnd, maxPoints);
	}

	public IEnumerable<Vector3Int> GetLine(Vector3Int start, Vector3Int end, LineDirection initialDirection, int maxPoints, out LineDirection direction)
	{
		if (start == end)
		{
			direction = LineDirection.SinglePoint;
			return Enumerables.One(start);
		}
		bool flag = ((initialDirection == LineDirection.Left || initialDirection == LineDirection.Right) ? true : false);
		int x = (flag ? end.x : start.x);
		flag = ((initialDirection == LineDirection.Down || initialDirection == LineDirection.Up) ? true : false);
		int y = (flag ? end.y : start.y);
		Vector3Int vector3Int = new Vector3Int(x, y, start.z);
		direction = ((start == vector3Int) ? GetLineDirection(start, end) : initialDirection);
		return GetCuboid(start, vector3Int, maxPoints);
	}

	public IEnumerable<Vector3Int> GetCuboid(Vector3Int start, Vector3Int end, int maxSize = 0)
	{
		Vector3Int clampedEnd = ((maxSize > 0) ? _areaClamper.ClampEnd(start, end, maxSize) : end);
		int deltaX = ((start.x < clampedEnd.x) ? 1 : (-1));
		int deltaY = ((start.y < clampedEnd.y) ? 1 : (-1));
		int deltaZ = ((start.z < clampedEnd.z) ? 1 : (-1));
		for (int x = start.x; x != clampedEnd.x + deltaX; x += deltaX)
		{
			for (int y = start.y; y != clampedEnd.y + deltaY; y += deltaY)
			{
				for (int z = start.z; z != clampedEnd.z + deltaZ; z += deltaZ)
				{
					yield return new Vector3Int(x, y, z);
				}
			}
		}
	}

	private static LineDirection GetLineDirection(Vector3Int start, Vector3Int end)
	{
		GetFurthestLineEnd(start, end, out var direction);
		return direction;
	}

	private static Vector3Int GetFurthestLineEnd(Vector3Int start, Vector3Int end, out LineDirection direction)
	{
		int num = end.x - start.x;
		int num2 = end.y - start.y;
		int num3 = Math.Abs(num);
		int num4 = Math.Abs(num2);
		Vector2Int vector2Int;
		if (num3 > num4)
		{
			vector2Int = new Vector2Int(end.x, start.y);
			direction = ((num > 0) ? LineDirection.Right : LineDirection.Left);
		}
		else
		{
			vector2Int = new Vector2Int(start.x, end.y);
			direction = ((num2 <= 0) ? LineDirection.Down : LineDirection.Up);
		}
		return new Vector3Int(vector2Int.x, vector2Int.y, start.z);
	}
}
