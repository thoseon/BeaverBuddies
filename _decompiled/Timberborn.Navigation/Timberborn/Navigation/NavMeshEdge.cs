using System;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Navigation;

public readonly struct NavMeshEdge : IEquatable<NavMeshEdge>
{
	public Vector3Int Start { get; }

	public Vector3Int End { get; }

	public int GroupId { get; }

	public bool IsRoad { get; }

	public float Cost { get; }

	private NavMeshEdge(Vector3Int start, Vector3Int end, int groupId, bool isRoad, float cost)
	{
		Start = start;
		End = end;
		GroupId = groupId;
		IsRoad = isRoad;
		Cost = cost;
	}

	public static NavMeshEdge CreateDefault(Vector3Int start, Vector3Int end)
	{
		float cost = Vector2Int.Distance(start.XY(), end.XY());
		return CreateGrouped(start, end, 0, isRoad: false, cost);
	}

	public static NavMeshEdge CreateBlocking(Vector3Int start, Vector3Int end, int groupId)
	{
		return CreateGrouped(start, end, groupId, isRoad: false, 0f);
	}

	public static NavMeshEdge CreateGrouped(Vector3Int start, Vector3Int end, int groupId, bool isRoad, float cost)
	{
		return new NavMeshEdge(start, end, groupId, isRoad, cost);
	}

	public bool Equals(NavMeshEdge other)
	{
		if (Start.Equals(other.Start) && End.Equals(other.End) && Cost.Equals(other.Cost) && IsRoad == other.IsRoad)
		{
			return GroupId == other.GroupId;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is NavMeshEdge other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((((Start.GetHashCode() * 397 * 397) ^ End.GetHashCode()) * 397) ^ GroupId.GetHashCode()) * 397) ^ IsRoad.GetHashCode()) * 397) ^ Cost.GetHashCode();
	}

	public static bool operator ==(NavMeshEdge left, NavMeshEdge right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(NavMeshEdge left, NavMeshEdge right)
	{
		return !left.Equals(right);
	}
}
