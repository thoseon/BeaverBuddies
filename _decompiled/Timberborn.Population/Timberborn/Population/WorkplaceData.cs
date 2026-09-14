using System;

namespace Timberborn.Population;

public readonly struct WorkplaceData : IEquatable<WorkplaceData>
{
	public int OccupiedWorkslots { get; }

	public int FreeWorkslots { get; }

	public int Unemployed { get; }

	public int TotalWorkslots => OccupiedWorkslots + FreeWorkslots;

	public WorkplaceData(int occupiedWorkslots, int freeWorkslots, int unemployed)
	{
		OccupiedWorkslots = occupiedWorkslots;
		FreeWorkslots = freeWorkslots;
		Unemployed = unemployed;
	}

	public bool Equals(WorkplaceData other)
	{
		if (OccupiedWorkslots == other.OccupiedWorkslots && FreeWorkslots == other.FreeWorkslots)
		{
			return Unemployed == other.Unemployed;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is WorkplaceData other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((OccupiedWorkslots * 397) ^ FreeWorkslots) * 397) ^ Unemployed;
	}

	public static bool operator ==(WorkplaceData left, WorkplaceData right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(WorkplaceData left, WorkplaceData right)
	{
		return !left.Equals(right);
	}
}
