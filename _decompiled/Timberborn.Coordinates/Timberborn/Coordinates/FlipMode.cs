using System;
using UnityEngine;

namespace Timberborn.Coordinates;

public readonly struct FlipMode : IEquatable<FlipMode>
{
	public static readonly FlipMode Unflipped = new FlipMode(isFlipped: false);

	public static readonly FlipMode Flipped = new FlipMode(isFlipped: true);

	public bool IsFlipped { get; }

	public bool IsUnflipped => !IsFlipped;

	public FlipMode(bool isFlipped)
	{
		IsFlipped = isFlipped;
	}

	public FlipMode Flip()
	{
		return new FlipMode(!IsFlipped);
	}

	public Vector3Int Transform(Vector3Int coordinates, int width)
	{
		if (!IsFlipped)
		{
			return coordinates;
		}
		return new Vector3Int(width - coordinates.x - 1, coordinates.y, coordinates.z);
	}

	public Vector2Int Transform(Vector2Int coordinates, int width)
	{
		if (!IsFlipped)
		{
			return coordinates;
		}
		return new Vector2Int(width - coordinates.x - 1, coordinates.y);
	}

	public Vector3 Transform(Vector3 coordinates, int width)
	{
		if (!IsFlipped)
		{
			return coordinates;
		}
		return new Vector3((float)width - coordinates.x, coordinates.y, coordinates.z);
	}

	public Direction2D Transform(Direction2D direction2D)
	{
		if (!IsFlipped || (direction2D != Direction2D.Left && direction2D != Direction2D.Right))
		{
			return direction2D;
		}
		return direction2D.Across();
	}

	public Direction3D Transform(Direction3D direction3D)
	{
		if (!IsFlipped || (direction3D != Direction3D.Left && direction3D != Direction3D.Right))
		{
			return direction3D;
		}
		return direction3D.Across();
	}

	public bool Equals(FlipMode other)
	{
		return IsFlipped == other.IsFlipped;
	}

	public override bool Equals(object obj)
	{
		if (obj is FlipMode other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return IsFlipped.GetHashCode();
	}

	public static bool operator ==(FlipMode left, FlipMode right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(FlipMode left, FlipMode right)
	{
		return !left.Equals(right);
	}
}
