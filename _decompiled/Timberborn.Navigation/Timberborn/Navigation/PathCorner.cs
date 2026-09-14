using System;
using UnityEngine;

namespace Timberborn.Navigation;

public readonly struct PathCorner : IEquatable<PathCorner>
{
	public Vector3 Position { get; }

	public float Speed { get; }

	public int GroupId { get; }

	public PathCorner(Vector3 position, float speed, int groupId)
	{
		Position = position;
		Speed = speed;
		GroupId = groupId;
	}

	public bool Equals(PathCorner other)
	{
		if (Position.Equals(other.Position) && Speed.Equals(other.Speed))
		{
			return GroupId == other.GroupId;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is PathCorner other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((Position.GetHashCode() * 397) ^ Speed.GetHashCode()) * 397) ^ GroupId.GetHashCode();
	}
}
