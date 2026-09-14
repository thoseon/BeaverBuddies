using System;
using UnityEngine;

namespace Timberborn.BlockObjectAccesses;

public readonly struct ParentedNeighbor : IEquatable<ParentedNeighbor>
{
	public Vector3Int Neighbor { get; }

	public Vector3Int Parent { get; }

	public ParentedNeighbor(Vector3Int neighbor, Vector3Int parent)
	{
		Neighbor = neighbor;
		Parent = parent;
	}

	public bool Equals(ParentedNeighbor other)
	{
		if (Neighbor.Equals(other.Neighbor))
		{
			return Parent.Equals(other.Parent);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is ParentedNeighbor other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Neighbor.GetHashCode() * 397) ^ Parent.GetHashCode();
	}

	public static bool operator ==(ParentedNeighbor left, ParentedNeighbor right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ParentedNeighbor left, ParentedNeighbor right)
	{
		return !left.Equals(right);
	}
}
