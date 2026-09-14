using System;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.BlockObjectAccesses;

public readonly struct ParentedNeighbor2D : IEquatable<ParentedNeighbor2D>
{
	public Vector2Int Neighbor { get; }

	public Vector2Int Parent { get; }

	private ParentedNeighbor2D(Vector2Int neighbor, Vector2Int parent)
	{
		Neighbor = neighbor;
		Parent = parent;
	}

	public static ParentedNeighbor2D From3D(ParentedNeighbor parentedNeighbor)
	{
		return new ParentedNeighbor2D(parentedNeighbor.Neighbor.XY(), parentedNeighbor.Parent.XY());
	}

	public static ParentedNeighbor2D FromVectors(Vector3Int neighbor, Vector3Int parent)
	{
		return new ParentedNeighbor2D(neighbor.XY(), parent.XY());
	}

	public bool Equals(ParentedNeighbor2D other)
	{
		if (Neighbor.Equals(other.Neighbor))
		{
			return Parent.Equals(other.Parent);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is ParentedNeighbor2D other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Neighbor.GetHashCode() * 397) ^ Parent.GetHashCode();
	}

	public static bool operator ==(ParentedNeighbor2D left, ParentedNeighbor2D right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ParentedNeighbor2D left, ParentedNeighbor2D right)
	{
		return !left.Equals(right);
	}
}
