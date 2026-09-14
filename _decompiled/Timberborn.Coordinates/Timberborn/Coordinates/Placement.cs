using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Timberborn.Coordinates;

public readonly struct Placement
{
	public static readonly Placement Zero;

	public Vector3Int Coordinates { get; }

	public Orientation Orientation { get; }

	public FlipMode FlipMode { get; }

	public Placement(Vector3Int coordinates)
	{
		Coordinates = coordinates;
		Orientation = Orientation.Cw0;
		FlipMode = FlipMode.Unflipped;
	}

	public Placement(Vector3Int coordinates, Orientation orientation, FlipMode flipMode)
	{
		Coordinates = coordinates;
		Orientation = orientation;
		FlipMode = flipMode;
	}

	[UsedImplicitly]
	public bool Equals(Placement other)
	{
		if (Coordinates.Equals(other.Coordinates) && Orientation == other.Orientation)
		{
			return FlipMode.Equals(other.FlipMode);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is Placement other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Coordinates, (int)Orientation, FlipMode);
	}

	public static bool operator ==(Placement left, Placement right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Placement left, Placement right)
	{
		return !left.Equals(right);
	}
}
