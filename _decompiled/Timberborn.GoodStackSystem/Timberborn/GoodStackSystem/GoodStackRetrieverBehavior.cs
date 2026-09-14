using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockSystem;
using Timberborn.Carrying;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Navigation;
using Timberborn.TemplateSystem;
using Timberborn.WorkSystem;
using UnityEngine;

namespace Timberborn.GoodStackSystem;

public class GoodStackRetrieverBehavior : Behavior, IAwakableComponent, IJobBehavior
{
	private readonly CarryAmountCalculator _carryAmountCalculator;

	private BehaviorAgent _behaviorAgent;

	private IGoodStackService _goodStackService;

	private Accessible _accessible;

	private Inventory _inventory;

	private Func<string, bool> _templateNameValidation;

	public GoodStackRetrieverBehavior(CarryAmountCalculator carryAmountCalculator)
	{
		_carryAmountCalculator = carryAmountCalculator;
	}

	public void Awake()
	{
		_behaviorAgent = GetComponent<BehaviorAgent>();
	}

	public Decision StartRetrieving(IGoodStackService goodStackService, Accessible accessible, Inventory inventory)
	{
		return StartRetrieving(goodStackService, accessible, inventory, (string _) => true);
	}

	public Decision StartRetrieving(IGoodStackService goodStackService, Accessible accessible, Inventory inventory, Func<string, bool> templateNameValidation)
	{
		_goodStackService = goodStackService;
		_accessible = accessible;
		_inventory = inventory;
		_templateNameValidation = templateNameValidation;
		return Decide(_behaviorAgent);
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		for (int i = 0; i < _goodStackService.GoodStacks.Count; i++)
		{
			GoodStack goodStack = _goodStackService.GoodStacks[i];
			if (Exists(goodStack) && _templateNameValidation(goodStack.GetComponent<TemplateSpec>().TemplateName) && IsReachable(goodStack) && RetrieveGoodStack(goodStack, agent))
			{
				return Decision.ReleaseNextTick();
			}
		}
		return Decision.ReleaseNow();
	}

	private static bool Exists(GoodStack goodStack)
	{
		if ((bool)goodStack)
		{
			return !goodStack.GetComponent<EntityComponent>().Deleted;
		}
		return false;
	}

	private bool IsReachable(GoodStack goodStack)
	{
		Vector3 worldCenterGrounded = goodStack.GetComponent<BlockObjectCenter>().WorldCenterGrounded;
		return _accessible.IsReachableByTerrain(worldCenterGrounded);
	}

	private bool RetrieveGoodStack(GoodStack goodStack, BehaviorAgent agent)
	{
		foreach (GoodAmount item in goodStack.Inventory.UnreservedStock())
		{
			if (_inventory.UnreservedCapacity(item.GoodId) > 0)
			{
				GoodCarrier component = agent.GetComponent<GoodCarrier>();
				GoodAmount goodAmount = _carryAmountCalculator.AmountToCarry(component.LiftingCapacity, item, _inventory);
				GoodReserver component2 = agent.GetComponent<GoodReserver>();
				component2.ReserveNotLessThanStockAmountConsuming(goodStack.Inventory, goodAmount);
				component2.ReserveCapacity(_inventory, goodAmount);
				return true;
			}
		}
		return false;
	}
}
