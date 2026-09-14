using System;

namespace Timberborn.Population;

public readonly struct ContaminationData : IEquatable<ContaminationData>
{
	public int ContaminatedAdults { get; }

	public int ContaminatedChildren { get; }

	public int ContaminatedTotal => ContaminatedAdults + ContaminatedChildren;

	public ContaminationData(int contaminatedAdults, int contaminatedChildren)
	{
		ContaminatedAdults = contaminatedAdults;
		ContaminatedChildren = contaminatedChildren;
	}

	public bool Equals(ContaminationData other)
	{
		if (ContaminatedAdults == other.ContaminatedAdults)
		{
			return ContaminatedChildren == other.ContaminatedChildren;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is ContaminationData other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (ContaminatedAdults * 397) ^ ContaminatedChildren;
	}

	public static bool operator ==(ContaminationData left, ContaminationData right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ContaminationData left, ContaminationData right)
	{
		return !left.Equals(right);
	}
}
