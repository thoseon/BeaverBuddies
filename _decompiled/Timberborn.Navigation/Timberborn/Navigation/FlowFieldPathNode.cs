using System;
using UnityEngine;

namespace Timberborn.Navigation;

public readonly struct FlowFieldPathNode : IEquatable<FlowFieldPathNode>
{
	public Vector3 Position { get; }

	public float Cost { get; }

	public float DistanceToNext { get; }

	public int GroupId { get; }

	public float NormalizedSpeed
	{
		get
		{
			if (Cost != 0f)
			{
				return DistanceToNext / Cost;
			}
			return float.MaxValue;
		}
	}

	public FlowFieldPathNode(Vector3 position, float cost, float distanceToNext, int groupId)
	{
		Position = position;
		Cost = cost;
		DistanceToNext = distanceToNext;
		GroupId = groupId;
	}

	public bool Equals(FlowFieldPathNode other)
	{
		if (Position.Equals(other.Position) && Cost.Equals(other.Cost) && DistanceToNext.Equals(other.DistanceToNext))
		{
			return GroupId == other.GroupId;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is FlowFieldPathNode other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((Position.GetHashCode() * 397 * 397) ^ Cost.GetHashCode()) * 397) ^ DistanceToNext.GetHashCode()) * 397) ^ GroupId.GetHashCode();
	}
}
