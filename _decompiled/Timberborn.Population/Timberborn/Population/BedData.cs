using System;

namespace Timberborn.Population;

public readonly struct BedData : IEquatable<BedData>
{
	public int OccupiedBeds { get; }

	public int FreeBeds { get; }

	public int Homeless { get; }

	public BedData(int occupiedBeds, int freeBeds, int homeless)
	{
		OccupiedBeds = occupiedBeds;
		FreeBeds = freeBeds;
		Homeless = homeless;
	}

	public bool Equals(BedData other)
	{
		if (OccupiedBeds == other.OccupiedBeds && FreeBeds == other.FreeBeds)
		{
			return Homeless == other.Homeless;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is BedData other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((OccupiedBeds * 397) ^ FreeBeds) * 397) ^ Homeless;
	}

	public static bool operator ==(BedData left, BedData right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(BedData left, BedData right)
	{
		return !left.Equals(right);
	}
}
