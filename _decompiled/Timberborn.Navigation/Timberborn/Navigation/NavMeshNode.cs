using System;

namespace Timberborn.Navigation;

internal readonly struct NavMeshNode : IEquatable<NavMeshNode>
{
	public int Id { get; }

	public int GroupId { get; }

	public float Cost { get; }

	public NavMeshNode(int id, int groupId, float cost)
	{
		Id = id;
		Cost = cost;
		GroupId = groupId;
	}

	public bool Equals(NavMeshNode other)
	{
		if (Id == other.Id && Cost.Equals(other.Cost))
		{
			return GroupId == other.GroupId;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is NavMeshNode other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((Id.GetHashCode() * 397 * 397) ^ GroupId.GetHashCode()) * 397) ^ Cost.GetHashCode();
	}
}
