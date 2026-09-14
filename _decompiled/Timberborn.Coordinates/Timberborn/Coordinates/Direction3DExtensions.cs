using System;
using UnityEngine;

namespace Timberborn.Coordinates;

public static class Direction3DExtensions
{
	private static readonly Vector3Int BottomVector = new Vector3Int(0, 0, -1);

	private static readonly Vector3Int TopVector = new Vector3Int(0, 0, 1);

	public static Vector3Int ToOffset(this Direction3D direction3D)
	{
		return direction3D switch
		{
			Direction3D.Down => Vector3Int.down, 
			Direction3D.Left => Vector3Int.left, 
			Direction3D.Up => Vector3Int.up, 
			Direction3D.Right => Vector3Int.right, 
			Direction3D.Bottom => BottomVector, 
			Direction3D.Top => TopVector, 
			_ => throw new ArgumentOutOfRangeException("direction3D", direction3D, null), 
		};
	}

	public static Direction3D FromOffset(Vector3Int offset)
	{
		if (offset == Vector3Int.down)
		{
			return Direction3D.Down;
		}
		if (offset == Vector3Int.left)
		{
			return Direction3D.Left;
		}
		if (offset == Vector3Int.up)
		{
			return Direction3D.Up;
		}
		if (offset == Vector3Int.right)
		{
			return Direction3D.Right;
		}
		if (offset == BottomVector)
		{
			return Direction3D.Bottom;
		}
		if (offset == TopVector)
		{
			return Direction3D.Top;
		}
		throw new ArgumentException("Can't create Direction3D " + string.Format("from {0} {1}", "offset", offset));
	}

	public static Directions3D ToDirections3D(this Direction3D direction3D)
	{
		return direction3D switch
		{
			Direction3D.Down => Directions3D.Down, 
			Direction3D.Left => Directions3D.Left, 
			Direction3D.Up => Directions3D.Up, 
			Direction3D.Right => Directions3D.Right, 
			Direction3D.Bottom => Directions3D.Bottom, 
			Direction3D.Top => Directions3D.Top, 
			_ => throw new ArgumentOutOfRangeException("direction3D", direction3D, null), 
		};
	}

	public static Direction3D RotateHorizontally(this Direction3D direction3D, Orientation orientation)
	{
		return orientation switch
		{
			Orientation.Cw0 => direction3D, 
			Orientation.Cw90 => direction3D.NextHorizontally(), 
			Orientation.Cw180 => direction3D.NextHorizontally().NextHorizontally(), 
			Orientation.Cw270 => direction3D.NextHorizontally().NextHorizontally().NextHorizontally(), 
			_ => throw new ArgumentOutOfRangeException("orientation", orientation, null), 
		};
	}

	public static float ToHorizontalAngle(this Direction3D direction3D)
	{
		switch (direction3D)
		{
		case Direction3D.Down:
		case Direction3D.Bottom:
		case Direction3D.Top:
			return 0f;
		case Direction3D.Left:
			return 90f;
		case Direction3D.Up:
			return 180f;
		case Direction3D.Right:
			return 270f;
		default:
			throw new ArgumentOutOfRangeException("direction3D", direction3D, null);
		}
	}

	public static bool IsHorizontal(this Direction3D direction3D)
	{
		if (direction3D != Direction3D.Down && direction3D != Direction3D.Up && direction3D != Direction3D.Left)
		{
			return direction3D == Direction3D.Right;
		}
		return true;
	}

	public static Direction3D Across(this Direction3D direction3D)
	{
		return direction3D switch
		{
			Direction3D.Down => Direction3D.Up, 
			Direction3D.Left => Direction3D.Right, 
			Direction3D.Up => Direction3D.Down, 
			Direction3D.Right => Direction3D.Left, 
			Direction3D.Bottom => Direction3D.Top, 
			Direction3D.Top => Direction3D.Bottom, 
			_ => throw new ArgumentOutOfRangeException("direction3D", direction3D, null), 
		};
	}

	public static Quaternion ToRotation(this Direction3D direction)
	{
		if (direction.IsHorizontal())
		{
			return Quaternion.AngleAxis(direction.ToHorizontalAngle(), Vector3.up);
		}
		if (direction != Direction3D.Top)
		{
			return Quaternion.AngleAxis(-90f, Vector3.right);
		}
		return Quaternion.AngleAxis(90f, Vector3.right);
	}

	private static Direction3D NextHorizontally(this Direction3D direction3D)
	{
		return direction3D switch
		{
			Direction3D.Down => Direction3D.Left, 
			Direction3D.Left => Direction3D.Up, 
			Direction3D.Up => Direction3D.Right, 
			Direction3D.Right => Direction3D.Down, 
			Direction3D.Bottom => Direction3D.Bottom, 
			Direction3D.Top => Direction3D.Top, 
			_ => throw new ArgumentOutOfRangeException("direction3D", direction3D, null), 
		};
	}
}
