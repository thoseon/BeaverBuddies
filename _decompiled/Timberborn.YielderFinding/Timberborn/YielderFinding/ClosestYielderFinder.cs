using System.Collections.Generic;
using Timberborn.Carrying;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Yielding;

namespace Timberborn.YielderFinding;

public class ClosestYielderFinder
{
	private readonly CarryAmountCalculator _carryAmountCalculator;

	private readonly Dictionary<string, ReachableYielder> _yielders = new Dictionary<string, ReachableYielder>();

	private readonly SortedSet<ReachableYielder> _orderedYielders = new SortedSet<ReachableYielder>();

	public ClosestYielderFinder(CarryAmountCalculator carryAmountCalculator)
	{
		_carryAmountCalculator = carryAmountCalculator;
	}

	public YielderSearchResult FindLivingYielder(Inventory receivingInventory, int liftingCapacity, IEnumerable<ReachableYielder> reachableYielders)
	{
		return FindYielder(receivingInventory, liftingCapacity, reachableYielders, isLiving: true);
	}

	public YielderSearchResult FindYielder(Inventory receivingInventory, int liftingCapacity, IEnumerable<ReachableYielder> reachableYielders)
	{
		return FindYielder(receivingInventory, liftingCapacity, reachableYielders, isLiving: false);
	}

	private YielderSearchResult FindYielder(Inventory receivingInventory, int liftingCapacity, IEnumerable<ReachableYielder> reachableYielders, bool isLiving)
	{
		if (!FindClosestYielders(reachableYielders, isLiving))
		{
			return YielderSearchResult.CreateNoYielderInRange();
		}
		YielderSearchResult result = FindYielder(receivingInventory, liftingCapacity);
		_yielders.Clear();
		_orderedYielders.Clear();
		return result;
	}

	private bool FindClosestYielders(IEnumerable<ReachableYielder> reachableYielders, bool isLiving)
	{
		bool flag = false;
		foreach (ReachableYielder reachableYielder in reachableYielders)
		{
			Yielder yielder = reachableYielder.Yielder;
			if ((bool)yielder)
			{
				bool isYielding = yielder.IsYielding;
				flag = (flag | isYielding) || (isLiving && yielder.IsAlive());
				if (isYielding)
				{
					AddCloserYielder(reachableYielder);
				}
			}
		}
		return flag;
	}

	private void AddCloserYielder(ReachableYielder reachableYielder)
	{
		string goodId = reachableYielder.Yielder.Yield.GoodId;
		if (_yielders.TryGetValue(goodId, out var value))
		{
			if (reachableYielder.Distance < value.Distance)
			{
				_yielders[goodId] = reachableYielder;
			}
		}
		else
		{
			_yielders[goodId] = reachableYielder;
		}
	}

	private YielderSearchResult FindYielder(Inventory receivingInventory, int liftingCapacity)
	{
		_orderedYielders.AddRange(_yielders.Values);
		foreach (ReachableYielder orderedYielder in _orderedYielders)
		{
			Yielder yielder = orderedYielder.Yielder;
			GoodAmount yield = _carryAmountCalculator.AmountToCarry(liftingCapacity, yielder.Yield, receivingInventory);
			if (yield.Amount > 0)
			{
				return YielderSearchResult.CreateSearchResult(yielder, yield);
			}
		}
		return YielderSearchResult.CreateEmpty();
	}
}
