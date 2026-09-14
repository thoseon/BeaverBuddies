using System;
using Timberborn.Yielding;

namespace Timberborn.YielderFinding;

public readonly struct ReachableYielder : IComparable<ReachableYielder>
{
	public Yielder Yielder { get; }

	public float Distance { get; }

	public ReachableYielder(Yielder yielder, float distance)
	{
		Yielder = yielder;
		Distance = distance;
	}

	public int CompareTo(ReachableYielder other)
	{
		int num = Distance.CompareTo(other.Distance);
		if (num != 0)
		{
			return num;
		}
		return Yielder.InstantiationOrder.CompareTo(other.Yielder.InstantiationOrder);
	}
}
