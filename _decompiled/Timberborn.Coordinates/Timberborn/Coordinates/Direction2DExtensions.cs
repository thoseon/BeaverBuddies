using System;
using UnityEngine;

namespace Timberborn.Coordinates;

public static class Direction2DExtensions
{
	public static Vector3Int ToOffset(this Direction2D direction2D)
	{
		return direction2D switch
		{
			Direction2D.Down => Vector3Int.down, 
			Direction2D.Left => Vector3Int.left, 
			Direction2D.Up => Vector3Int.up, 
			Direction2D.Right => Vector3Int.right, 
			_ => throw new ArgumentOutOfRangeException("direction2D", direction2D, null), 
		};
	}

	public static Direction2D Next(this Direction2D direction2D)
	{
		return direction2D switch
		{
			Direction2D.Down => Direction2D.Left, 
			Direction2D.Left => Direction2D.Up, 
			Direction2D.Up => Direction2D.Right, 
			Direction2D.Right => Direction2D.Down, 
			_ => throw new ArgumentOutOfRangeException("direction2D", direction2D, null), 
		};
	}

	public static Direction2D Across(this Direction2D direction2D)
	{
		return direction2D switch
		{
			Direction2D.Down => Direction2D.Up, 
			Direction2D.Left => Direction2D.Right, 
			Direction2D.Up => Direction2D.Down, 
			Direction2D.Right => Direction2D.Left, 
			_ => throw new ArgumentOutOfRangeException("direction2D", direction2D, null), 
		};
	}

	public static Directions2D ToDirections(this Direction2D direction2D)
	{
		return direction2D switch
		{
			Direction2D.Down => Directions2D.Down, 
			Direction2D.Left => Directions2D.Left, 
			Direction2D.Up => Directions2D.Up, 
			Direction2D.Right => Directions2D.Right, 
			_ => throw new ArgumentOutOfRangeException("direction2D", direction2D, null), 
		};
	}

	public static Directions3D ToDirections3D(this Direction2D direction2D)
	{
		return direction2D switch
		{
			Direction2D.Down => Directions3D.Down, 
			Direction2D.Left => Directions3D.Left, 
			Direction2D.Up => Directions3D.Up, 
			Direction2D.Right => Directions3D.Right, 
			_ => throw new ArgumentOutOfRangeException("direction2D", direction2D, null), 
		};
	}

	public static float ToAngle(this Direction2D direction2D)
	{
		return direction2D switch
		{
			Direction2D.Down => 0f, 
			Direction2D.Left => 90f, 
			Direction2D.Up => 180f, 
			Direction2D.Right => 270f, 
			_ => throw new ArgumentOutOfRangeException("direction2D", direction2D, null), 
		};
	}

	public static Orientation ToOrientation(this Direction2D direction2D)
	{
		return direction2D switch
		{
			Direction2D.Down => Orientation.Cw0, 
			Direction2D.Left => Orientation.Cw90, 
			Direction2D.Up => Orientation.Cw180, 
			Direction2D.Right => Orientation.Cw270, 
			_ => throw new ArgumentOutOfRangeException("direction2D", direction2D, null), 
		};
	}

	public static Quaternion ToWorldSpaceRotation(this Direction2D direction2D)
	{
		return Quaternion.AngleAxis(direction2D.ToAngle(), CoordinateSystem.GridToWorld(new Vector3(0f, 0f, 1f)));
	}
}
