using System;

namespace Timberborn.EntityNaming;

public readonly struct NamedEntitySortingKey : IEquatable<NamedEntitySortingKey>, IComparable<NamedEntitySortingKey>
{
	private string SortableName { get; }

	private Guid EntityId { get; }

	internal NamedEntitySortingKey(string sortableName, Guid entityId)
	{
		SortableName = sortableName;
		EntityId = entityId;
	}

	public bool Equals(NamedEntitySortingKey other)
	{
		if (SortableName == other.SortableName)
		{
			return EntityId.Equals(other.EntityId);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is NamedEntitySortingKey other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(SortableName, EntityId);
	}

	public int CompareTo(NamedEntitySortingKey other)
	{
		int num = string.Compare(SortableName, other.SortableName, StringComparison.InvariantCulture);
		if (num == 0)
		{
			return EntityId.CompareTo(other.EntityId);
		}
		return num;
	}

	public static bool operator ==(NamedEntitySortingKey left, NamedEntitySortingKey right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(NamedEntitySortingKey left, NamedEntitySortingKey right)
	{
		return !left.Equals(right);
	}
}
