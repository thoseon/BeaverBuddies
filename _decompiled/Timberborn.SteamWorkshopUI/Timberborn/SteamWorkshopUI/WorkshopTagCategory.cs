using System;

namespace Timberborn.SteamWorkshopUI;

public readonly struct WorkshopTagCategory : IEquatable<WorkshopTagCategory>
{
	public string Name { get; }

	public int Order { get; }

	public WorkshopTagCategory(string name, int order)
	{
		Name = name;
		Order = order;
	}

	public bool Equals(WorkshopTagCategory other)
	{
		if (Name == other.Name)
		{
			return Order == other.Order;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is WorkshopTagCategory other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((Name != null) ? Name.GetHashCode() : 0) * 397) ^ Order;
	}

	public static bool operator ==(WorkshopTagCategory left, WorkshopTagCategory right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(WorkshopTagCategory left, WorkshopTagCategory right)
	{
		return !left.Equals(right);
	}
}
