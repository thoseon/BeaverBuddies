using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Carrying;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Navigation;

namespace Timberborn.Emptying;

public class EmptyingStarter : BaseComponent, IAwakableComponent
{
	private readonly CarryAmountCalculator _carryAmountCalculator;

	private CarrierInventoryFinder _carrierInventoryFinder;

	private GoodCarrier _goodCarrier;

	private GoodReserver _goodReserver;

	public EmptyingStarter(CarryAmountCalculator carryAmountCalculator)
	{
		_carryAmountCalculator = carryAmountCalculator;
	}

	public void Awake()
	{
		_carrierInventoryFinder = GetComponent<CarrierInventoryFinder>();
		_goodCarrier = GetComponent<GoodCarrier>();
		_goodReserver = GetComponent<GoodReserver>();
	}

	public bool StartEmptying(Inventory inventory)
	{
		if (!inventory.IsEmpty)
		{
			return StartEmptying(inventory, unwantedStock: false);
		}
		return false;
	}

	public bool StartEmptyingUnwantedStock(Inventory inventory)
	{
		if (!inventory.IsEmpty)
		{
			return StartEmptying(inventory, unwantedStock: true);
		}
		return false;
	}

	private bool StartEmptying(Inventory inventory, bool unwantedStock)
	{
		IEnumerable<GoodAmount> goods = (unwantedStock ? inventory.UnreservedUnwantedStock() : GetUnreservedGoods(inventory));
		var (goodAmount, inventory2) = GetCarriableGood(inventory, goods);
		if (goodAmount.Amount > 0)
		{
			if (unwantedStock)
			{
				_goodReserver.ReserveExactStockAmount(inventory, goodAmount);
			}
			else
			{
				_goodReserver.ReserveNotLessThanStockAmount(inventory, goodAmount);
			}
			_goodReserver.ReserveCapacity(inventory2, goodAmount);
			return true;
		}
		return false;
	}

	private (GoodAmount, Inventory) GetCarriableGood(Inventory inventory, IEnumerable<GoodAmount> goods)
	{
		foreach (GoodAmount good in goods)
		{
			Accessible enabledComponent = inventory.GetEnabledComponent<Accessible>();
			Inventory closestInventoryWithCapacity = _carrierInventoryFinder.GetClosestInventoryWithCapacity(good.GoodId, enabledComponent, out var _);
			if (closestInventoryWithCapacity != null)
			{
				GoodAmount item = _carryAmountCalculator.AmountToCarry(_goodCarrier.LiftingCapacity, good, closestInventoryWithCapacity);
				if (item.Amount > 0)
				{
					return (item, closestInventoryWithCapacity);
				}
			}
		}
		return default((GoodAmount, Inventory));
	}

	private static IEnumerable<GoodAmount> GetUnreservedGoods(Inventory inventory)
	{
		Emptiable component = inventory.GetComponent<Emptiable>();
		if (!component || !component.IsMarkedForEmptying)
		{
			return inventory.UnreservedTakeableStock();
		}
		return inventory.UnreservedStock();
	}
}
