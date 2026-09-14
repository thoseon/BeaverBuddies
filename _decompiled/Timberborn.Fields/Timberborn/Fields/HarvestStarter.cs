using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Carrying;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Navigation;
using Timberborn.WorkSystem;
using Timberborn.YielderFinding;
using Timberborn.Yielding;

namespace Timberborn.Fields;

public class HarvestStarter : BaseComponent, IAwakableComponent
{
	private readonly YielderFinder _yielderFinder;

	private GoodReserver _goodReserver;

	private YielderRemover _yieldRemover;

	private YieldRemoverBehavior _yieldRemoverBehavior;

	private Worker _worker;

	private GoodCarrier _goodCarrier;

	private BehaviorAgent _behaviorAgent;

	private readonly List<Yielder> _yieldersCache = new List<Yielder>();

	public HarvestStarter(YielderFinder yielderFinder)
	{
		_yielderFinder = yielderFinder;
	}

	public void Awake()
	{
		_goodReserver = GetComponent<GoodReserver>();
		_yieldRemover = GetComponent<YielderRemover>();
		_yieldRemoverBehavior = GetComponent<YieldRemoverBehavior>();
		_worker = GetComponent<Worker>();
		_goodCarrier = GetComponent<GoodCarrier>();
		_behaviorAgent = GetComponent<BehaviorAgent>();
	}

	public Decision StartHarvesting(Inventory receivingInventory, InRangeYielders inRangeYielders, string prioritizedName)
	{
		YielderSearchResult yielderSearchResult2;
		if (!string.IsNullOrWhiteSpace(prioritizedName))
		{
			YielderSearchResult yielderSearchResult = FindYielder(receivingInventory, inRangeYielders, prioritizedName);
			if (yielderSearchResult.HasYielder)
			{
				yielderSearchResult2 = yielderSearchResult;
				goto IL_0027;
			}
		}
		yielderSearchResult2 = FindYielder(receivingInventory, inRangeYielders, null);
		goto IL_0027;
		IL_0027:
		YielderSearchResult searchResult = yielderSearchResult2;
		return StartHarvesting(receivingInventory, searchResult);
	}

	private Decision StartHarvesting(Inventory receivingInventory, YielderSearchResult searchResult)
	{
		if (searchResult.HasYielder)
		{
			GoodAmount yield = searchResult.Yield;
			searchResult.Yielder.Reservable.Reserve();
			_goodReserver.ReserveCapacity(receivingInventory, yield);
			_yieldRemover.ReserveForRemoval(searchResult.Yielder, yield);
			Decision decision = _yieldRemoverBehavior.Decide(_behaviorAgent);
			if (!decision.ShouldReleaseNow)
			{
				return Decision.TransferNow(_yieldRemoverBehavior, in decision);
			}
		}
		return Decision.ReleaseNow();
	}

	private YielderSearchResult FindYielder(Inventory receivingInventory, InRangeYielders inRangeYielders, string prioritizedName)
	{
		inRangeYielders.GetYielders(_yieldersCache, prioritizedName);
		Accessible enabledComponent = _worker.Workplace.GetEnabledComponent<Accessible>();
		int liftingCapacity = _goodCarrier.LiftingCapacity;
		YielderSearchResult result = _yielderFinder.FindLivingYielderWithoutAccessible(receivingInventory, enabledComponent, liftingCapacity, _yieldersCache);
		_yieldersCache.Clear();
		return result;
	}
}
