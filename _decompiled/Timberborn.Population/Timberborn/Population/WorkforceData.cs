using System;

namespace Timberborn.Population;

public readonly struct WorkforceData : IEquatable<WorkforceData>
{
	public int Employable { get; }

	public int Unemployable { get; }

	public int Total => Employable + Unemployable;

	public WorkforceData(int employable, int unemployable)
	{
		Employable = employable;
		Unemployable = unemployable;
	}

	public bool Equals(WorkforceData other)
	{
		if (Employable == other.Employable)
		{
			return Unemployable == other.Unemployable;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is WorkforceData other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Employable * 397) ^ Unemployable;
	}

	public static bool operator ==(WorkforceData left, WorkforceData right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(WorkforceData left, WorkforceData right)
	{
		return !left.Equals(right);
	}
}
