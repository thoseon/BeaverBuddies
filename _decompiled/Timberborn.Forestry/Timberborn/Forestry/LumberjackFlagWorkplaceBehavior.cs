using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Buildings;
using Timberborn.Carrying;
using Timberborn.EntitySystem;
using Timberborn.GoodStackSystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Navigation;
using Timberborn.SimpleOutputBuildings;
using Timberborn.WorkSystem;
using Timberborn.YielderFinding;
using Timberborn.Yielding;

namespace Timberborn.Forestry;

internal class LumberjackFlagWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent, IInitializableEntity
{
	private readonly YielderFinder _yielderFinder;

	private readonly TreeCuttingArea _treeCuttingArea;

	private readonly GoodStackService<LumberjackFlagSpec> _goodStackService;

	private YieldStatus _yieldStatus;

	private Inventory _inventory;

	private Accessible _accessible;

	public LumberjackFlagWorkplaceBehavior(YielderFinder yielderFinder, TreeCuttingArea treeCuttingArea, GoodStackService<LumberjackFlagSpec> goodStackService)
	{
		_yielderFinder = yielderFinder;
		_treeCuttingArea = treeCuttingArea;
		_goodStackService = goodStackService;
	}

	public void Awake()
	{
		_yieldStatus = GetComponent<YieldStatus>();
		_inventory = GetComponent<SimpleOutputInventory>().Inventory;
	}

	public void InitializeEntity()
	{
		_accessible = GetComponent<BuildingAccessible>().Accessible;
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		GoodStackRetrieverBehavior component = agent.GetComponent<GoodStackRetrieverBehavior>();
		Decision decision = component.StartRetrieving(_goodStackService, _accessible, _inventory);
		if (!decision.ShouldReleaseNow)
		{
			return Decision.TransferNow(component, in decision);
		}
		GoodCarrier component2 = agent.GetComponent<GoodCarrier>();
		YielderSearchResult yielderSearchResult = FindCuttable(component2.LiftingCapacity);
		if (yielderSearchResult.HasYielder)
		{
			GoodAmount yield = yielderSearchResult.Yield;
			yielderSearchResult.Yielder.Reservable.Reserve();
			agent.GetComponent<GoodReserver>().ReserveCapacity(_inventory, yield);
			agent.GetComponent<YielderRemover>().ReserveForRemoval(yielderSearchResult.Yielder, yield);
			YieldRemoverBehavior component3 = agent.GetComponent<YieldRemoverBehavior>();
			Decision decision2 = component3.Decide(agent);
			if (!decision2.ShouldReleaseNow)
			{
				return Decision.TransferNow(component3, in decision2);
			}
		}
		return Decision.ReleaseNow();
	}

	private YielderSearchResult FindCuttable(int liftingCapacity)
	{
		IEnumerable<Yielder> yielders = _treeCuttingArea.YieldersInArea.Where((Yielder yielder) => !yielder.Reservable.Reserved);
		YielderSearchResult yielderSearchResult = _yielderFinder.FindLivingYielderWithoutAccessible(_inventory, _accessible, liftingCapacity, yielders);
		_yieldStatus.UpdateStatus(yielderSearchResult);
		return yielderSearchResult;
	}
}
