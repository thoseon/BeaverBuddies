using System;
using UnityEngine;

namespace Timberborn.Navigation;

public readonly struct WeightedCoordinates : IEquatable<WeightedCoordinates>
{
	public Vector3Int Coordinates { get; }

	public float Distance { get; }

	public WeightedCoordinates(Vector3Int coordinates, float distance)
	{
		Coordinates = coordinates;
		Distance = distance;
	}

	public bool Equals(WeightedCoordinates other)
	{
		return Coordinates.Equals(other.Coordinates);
	}

	public override bool Equals(object obj)
	{
		if (obj is WeightedCoordinates other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Coordinates.GetHashCode();
	}

	public static bool operator ==(WeightedCoordinates left, WeightedCoordinates right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(WeightedCoordinates left, WeightedCoordinates right)
	{
		return !left.Equals(right);
	}
}
