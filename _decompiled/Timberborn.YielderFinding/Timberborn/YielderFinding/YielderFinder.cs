using System.Collections.Generic;
using System.Linq;
using Timberborn.InventorySystem;
using Timberborn.Navigation;
using Timberborn.Yielding;

namespace Timberborn.YielderFinding;

public class YielderFinder
{
	private readonly ClosestYielderFinder _closestYielderFinder;

	public YielderFinder(ClosestYielderFinder closestYielderFinder)
	{
		_closestYielderFinder = closestYielderFinder;
	}

	public YielderSearchResult FindLivingYielderWithoutAccessible(Inventory receivingInventory, Accessible start, int liftingCapacity, IEnumerable<Yielder> yielders)
	{
		IEnumerable<ReachableYielder> reachableYielders = yielders.Select((Yielder yielder) => RegularYielderAsReachable(start, yielder));
		return _closestYielderFinder.FindLivingYielder(receivingInventory, liftingCapacity, reachableYielders);
	}

	public YielderSearchResult FindYielderWithAccessible(Inventory receivingInventory, Accessible start, int liftingCapacity, IEnumerable<Yielder> yielders)
	{
		IEnumerable<ReachableYielder> reachableYielders = yielders.Select((Yielder yielder) => AccessibleYielderAsReachable(start, yielder));
		return _closestYielderFinder.FindYielder(receivingInventory, liftingCapacity, reachableYielders);
	}

	private static ReachableYielder RegularYielderAsReachable(Accessible start, Yielder yielder)
	{
		if (!start.FindTerrainPath(yielder.CenterPosition, out var distance))
		{
			return default(ReachableYielder);
		}
		return new ReachableYielder(yielder, distance);
	}

	private static ReachableYielder AccessibleYielderAsReachable(Accessible start, Yielder yielder)
	{
		Accessible enabledComponent = yielder.GetEnabledComponent<Accessible>();
		if (!start.FindTerrainPath(enabledComponent, out var distance))
		{
			return default(ReachableYielder);
		}
		return new ReachableYielder(yielder, distance);
	}
}
