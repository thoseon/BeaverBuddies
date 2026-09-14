using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Coordinates;

public static class OrientationExtensions
{
	public static float ToAngle(this Orientation orientation)
	{
		return orientation switch
		{
			Orientation.Cw0 => 0f, 
			Orientation.Cw90 => 90f, 
			Orientation.Cw180 => 180f, 
			Orientation.Cw270 => 270f, 
			_ => throw new ArgumentException(string.Format("Unexpected {0}: {1}", "Orientation", orientation)), 
		};
	}

	public static Quaternion ToWorldSpaceRotation(this Orientation orientation)
	{
		return Quaternion.AngleAxis(orientation.ToAngle(), CoordinateSystem.GridToWorld(new Vector3(0f, 0f, 1f)));
	}

	public static Vector3 ToPivotOffset(this Orientation orientation)
	{
		return orientation switch
		{
			Orientation.Cw0 => new Vector3(0f, 0f, 0f), 
			Orientation.Cw90 => new Vector3(0f, 1f, 0f), 
			Orientation.Cw180 => new Vector3(1f, 1f, 0f), 
			Orientation.Cw270 => new Vector3(1f, 0f, 0f), 
			_ => throw new ArgumentException(string.Format("Unexpected {0}: {1}", "Orientation", orientation)), 
		};
	}

	public static Orientation RotateClockwise(this Orientation orientation)
	{
		return orientation switch
		{
			Orientation.Cw0 => Orientation.Cw90, 
			Orientation.Cw90 => Orientation.Cw180, 
			Orientation.Cw180 => Orientation.Cw270, 
			Orientation.Cw270 => Orientation.Cw0, 
			_ => throw new ArgumentException(string.Format("Unexpected {0}: {1}", "Orientation", orientation)), 
		};
	}

	public static Orientation RotateCounterclockwise(this Orientation orientation)
	{
		return orientation switch
		{
			Orientation.Cw0 => Orientation.Cw270, 
			Orientation.Cw90 => Orientation.Cw0, 
			Orientation.Cw180 => Orientation.Cw90, 
			Orientation.Cw270 => Orientation.Cw180, 
			_ => throw new ArgumentException(string.Format("Unexpected {0}: {1}", "Orientation", orientation)), 
		};
	}

	public static Orientation Flip(this Orientation orientation)
	{
		return orientation.RotateClockwise().RotateClockwise();
	}

	public static Vector3 Transform(this Orientation orientation, Vector3 vector)
	{
		return orientation switch
		{
			Orientation.Cw0 => vector, 
			Orientation.Cw90 => new Vector3(vector.y, 0f - vector.x, vector.z), 
			Orientation.Cw180 => new Vector3(0f - vector.x, 0f - vector.y, vector.z), 
			Orientation.Cw270 => new Vector3(0f - vector.y, vector.x, vector.z), 
			_ => throw new ArgumentException(string.Format("Unexpected {0}: {1}", "Orientation", orientation)), 
		};
	}

	public static Vector3 TransformInWorldSpace(this Orientation orientation, Vector3 vector)
	{
		return orientation switch
		{
			Orientation.Cw0 => vector, 
			Orientation.Cw90 => new Vector3(vector.z, vector.y, 0f - vector.x), 
			Orientation.Cw180 => new Vector3(0f - vector.x, vector.y, 0f - vector.z), 
			Orientation.Cw270 => new Vector3(0f - vector.z, vector.y, vector.x), 
			_ => throw new ArgumentException(string.Format("Unexpected {0}: {1}", "Orientation", orientation)), 
		};
	}

	public static Vector3Int Transform(this Orientation orientation, Vector3Int vector)
	{
		return orientation switch
		{
			Orientation.Cw0 => vector, 
			Orientation.Cw90 => new Vector3Int(vector.y, -vector.x, vector.z), 
			Orientation.Cw180 => new Vector3Int(-vector.x, -vector.y, vector.z), 
			Orientation.Cw270 => new Vector3Int(-vector.y, vector.x, vector.z), 
			_ => throw new ArgumentException(string.Format("Unexpected {0}: {1}", "Orientation", orientation)), 
		};
	}

	public static Vector2Int Transform(this Orientation orientation, Vector2Int vector)
	{
		return orientation switch
		{
			Orientation.Cw0 => vector, 
			Orientation.Cw90 => new Vector2Int(vector.y, -vector.x), 
			Orientation.Cw180 => new Vector2Int(-vector.x, -vector.y), 
			Orientation.Cw270 => new Vector2Int(-vector.y, vector.x), 
			_ => throw new ArgumentException(string.Format("Unexpected {0}: {1}", "Orientation", orientation)), 
		};
	}

	public static Direction2D Transform(this Orientation orientation, Direction2D direction2D)
	{
		return orientation switch
		{
			Orientation.Cw0 => direction2D, 
			Orientation.Cw90 => direction2D.Next(), 
			Orientation.Cw180 => direction2D.Next().Next(), 
			Orientation.Cw270 => direction2D.Next().Next().Next(), 
			_ => throw new ArgumentException(string.Format("Unexpected {0}: {1}", "Orientation", orientation)), 
		};
	}

	public static IEnumerable<Orientation> AllValues()
	{
		return (Orientation[])Enum.GetValues(typeof(Orientation));
	}
}
