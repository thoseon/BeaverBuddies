using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Buildings;
using Timberborn.Carrying;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Navigation;
using Timberborn.SimpleOutputBuildings;
using Timberborn.WorkSystem;
using Timberborn.YielderFinding;
using Timberborn.Yielding;

namespace Timberborn.Ruins;

internal class ScavengerWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent, IInitializableEntity
{
	private readonly YielderFinder _yielderFinder;

	private Accessible _accessible;

	private YieldStatus _yieldStatus;

	private Inventory _inventory;

	private InRangeYielders _inRangeYielders;

	private readonly List<Yielder> _yieldersCache = new List<Yielder>();

	public ScavengerWorkplaceBehavior(YielderFinder yielderFinder)
	{
		_yielderFinder = yielderFinder;
	}

	public void Awake()
	{
		_yieldStatus = GetComponent<YieldStatus>();
		_inventory = GetComponent<SimpleOutputInventory>().Inventory;
		_inRangeYielders = GetComponent<InRangeYielders>();
	}

	public void InitializeEntity()
	{
		_accessible = GetComponent<BuildingAccessible>().Accessible;
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		GoodCarrier component = agent.GetComponent<GoodCarrier>();
		YielderSearchResult yielderSearchResult = FindYielder(_accessible, component.LiftingCapacity);
		_yieldStatus.UpdateStatus(yielderSearchResult);
		if (yielderSearchResult.HasYielder)
		{
			GoodAmount yield = yielderSearchResult.Yield;
			yielderSearchResult.Yielder.Reservable.Reserve();
			agent.GetComponent<GoodReserver>().ReserveCapacity(_inventory, yield);
			agent.GetComponent<YielderRemover>().ReserveForRemoval(yielderSearchResult.Yielder, yield);
			YieldRemoverBehavior component2 = agent.GetComponent<YieldRemoverBehavior>();
			Decision decision = component2.Decide(agent);
			if (!decision.ShouldReleaseNow)
			{
				return Decision.TransferNow(component2, in decision);
			}
		}
		return Decision.ReleaseNow();
	}

	private YielderSearchResult FindYielder(Accessible start, int liftingCapacity)
	{
		bool yielders = _inRangeYielders.GetYielders(_yieldersCache);
		if (_yieldersCache.Count > 0)
		{
			YielderSearchResult result = _yielderFinder.FindYielderWithAccessible(_inventory, start, liftingCapacity, _yieldersCache);
			_yieldersCache.Clear();
			return result;
		}
		if (!yielders)
		{
			return YielderSearchResult.CreateNoYielderInRange();
		}
		return YielderSearchResult.CreateEmpty();
	}
}
