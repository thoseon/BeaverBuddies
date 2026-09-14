using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Carrying;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Navigation;
using Timberborn.WorkSystem;

namespace Timberborn.RecoveredGoodSystem;

internal class RecoveredGoodStackCarryingBehavior : Behavior, IAwakableComponent
{
	private readonly CarryAmountCalculator _carryAmountCalculator;

	private CarrierInventoryFinder _carrierInventoryFinder;

	private BehaviorAgent _behaviorAgent;

	private RecoveredGoodStack _recoveredGoodStack;

	private Accessible _inventoryAccessible;

	private Inventory _targetInventory;

	public RecoveredGoodStackCarryingBehavior(CarryAmountCalculator carryAmountCalculator)
	{
		_carryAmountCalculator = carryAmountCalculator;
	}

	public void Awake()
	{
		_carrierInventoryFinder = GetComponent<CarrierInventoryFinder>();
		_behaviorAgent = GetComponent<BehaviorAgent>();
	}

	public Decision FindInventoryAndStartCarrying(RecoveredGoodStack recoveredGoodStack)
	{
		Accessible accessible = recoveredGoodStack.GetComponent<RecoveredGoodStackAccessible>().Accessible;
		NoStorageStatus component = recoveredGoodStack.GetComponent<NoStorageStatus>();
		foreach (GoodAmount item in recoveredGoodStack.Inventory.Stock)
		{
			Inventory inventory = FindBestInventory(accessible, item);
			if (inventory != null)
			{
				Accessible enabledComponent = inventory.GetEnabledComponent<Accessible>();
				Decision result = StartCarrying(recoveredGoodStack, enabledComponent, inventory);
				if (!result.ShouldReleaseNow)
				{
					component.DeactivateNoStorageStatus();
					return result;
				}
			}
		}
		component.ActivateNoStorageStatus();
		return Decision.ReleaseNow();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (Exists() && IsReachable() && RetrieveGoodStack(agent))
		{
			return Decision.ReleaseNextTick();
		}
		return Decision.ReleaseNow();
	}

	private Inventory FindBestInventory(Accessible start, GoodAmount goodAmount)
	{
		Inventory inventory = _carrierInventoryFinder.GetClosestInventoryWithCapacity(goodAmount.GoodId, start, out var distance);
		Workplace workplace = _behaviorAgent.GetComponent<Worker>().Workplace;
		Inventory enabledComponent = workplace.GetEnabledComponent<Inventory>();
		if (enabledComponent.UnreservedCapacity(goodAmount.GoodId) > 0)
		{
			Accessible enabledComponent2 = workplace.GetEnabledComponent<Accessible>();
			if ((enabledComponent2.FindRoadPath(start, out var distance2) || enabledComponent2.FindRoadToTerrainPath(start.Transform.position, out distance2)) && (inventory == null || distance2 < distance))
			{
				inventory = enabledComponent;
			}
		}
		return inventory;
	}

	private Decision StartCarrying(RecoveredGoodStack recoveredGoodStack, Accessible inventoryAccessible, Inventory targetInventory)
	{
		_recoveredGoodStack = recoveredGoodStack;
		_inventoryAccessible = inventoryAccessible;
		_targetInventory = targetInventory;
		return Decide(_behaviorAgent);
	}

	private bool Exists()
	{
		if ((bool)_recoveredGoodStack)
		{
			return !_recoveredGoodStack.GetComponent<EntityComponent>().Deleted;
		}
		return false;
	}

	private bool IsReachable()
	{
		return _inventoryAccessible.IsReachableByRoadToTerrain(_recoveredGoodStack.GetEnabledComponent<Accessible>());
	}

	private bool RetrieveGoodStack(BehaviorAgent agent)
	{
		foreach (GoodAmount item in _recoveredGoodStack.Inventory.UnreservedStock())
		{
			if (_targetInventory.UnreservedCapacity(item.GoodId) > 0)
			{
				GoodCarrier component = agent.GetComponent<GoodCarrier>();
				GoodAmount goodAmount = _carryAmountCalculator.AmountToCarry(component.LiftingCapacity, item, _targetInventory);
				GoodReserver component2 = agent.GetComponent<GoodReserver>();
				component2.ReserveNotLessThanStockAmount(_recoveredGoodStack.Inventory, goodAmount);
				component2.ReserveCapacity(_targetInventory, goodAmount);
				return true;
			}
		}
		return false;
	}
}
