using System.Collections.Generic;
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
using Timberborn.TemplateSystem;
using Timberborn.WorkSystem;
using Timberborn.YielderFinding;
using Timberborn.Yielding;

namespace Timberborn.Gathering;

internal class GatherWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent, IInitializableEntity
{
	private readonly YielderFinder _yielderFinder;

	private readonly GoodStackService<GathererFlag> _goodStackService;

	private Accessible _accessible;

	private GatherablePrioritizer _gatherablePrioritizer;

	private YieldStatus _yieldStatus;

	private Inventory _inventory;

	private GathererFlag _gathererFlag;

	private InRangeYielders _inRangeYielders;

	private readonly List<Yielder> _yieldersCache = new List<Yielder>();

	public GatherWorkplaceBehavior(YielderFinder yielderFinder, GoodStackService<GathererFlag> goodStackService)
	{
		_yielderFinder = yielderFinder;
		_goodStackService = goodStackService;
	}

	public void Awake()
	{
		_gatherablePrioritizer = GetComponent<GatherablePrioritizer>();
		_yieldStatus = GetComponent<YieldStatus>();
		_inventory = GetComponent<SimpleOutputInventory>().Inventory;
		_gathererFlag = GetComponent<GathererFlag>();
		_inRangeYielders = GetComponent<InRangeYielders>();
	}

	public void InitializeEntity()
	{
		_accessible = GetComponent<BuildingAccessible>().Accessible;
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		GoodStackRetrieverBehavior component = agent.GetComponent<GoodStackRetrieverBehavior>();
		Decision decision = component.StartRetrieving(_goodStackService, _accessible, _inventory, _gathererFlag.CanGather);
		if (!decision.ShouldReleaseNow)
		{
			return Decision.TransferNow(component, in decision);
		}
		GoodCarrier component2 = agent.GetComponent<GoodCarrier>();
		YielderSearchResult yielderSearchResult = FindYielder(_accessible, component2.LiftingCapacity, _gatherablePrioritizer.PrioritizedGatherable);
		_yieldStatus.UpdateStatus(yielderSearchResult);
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

	private YielderSearchResult FindYielder(Accessible start, int liftingCapacity, GatherableSpec gatherableSpec)
	{
		if (gatherableSpec != null)
		{
			string templateName = gatherableSpec.GetSpec<TemplateSpec>().TemplateName;
			YielderSearchResult result = FindYielder(start, liftingCapacity, templateName);
			if (result.HasYielder)
			{
				return result;
			}
		}
		return FindYielder(start, liftingCapacity);
	}

	private YielderSearchResult FindYielder(Accessible start, int liftingCapacity, string templateName = null)
	{
		bool yielders = _inRangeYielders.GetYielders(_yieldersCache, templateName);
		if (_yieldersCache.Count > 0)
		{
			YielderSearchResult result = _yielderFinder.FindLivingYielderWithoutAccessible(_inventory, start, liftingCapacity, _yieldersCache);
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
