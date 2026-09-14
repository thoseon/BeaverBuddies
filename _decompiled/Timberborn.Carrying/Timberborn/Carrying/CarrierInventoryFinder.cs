using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterNavigation;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Navigation;

namespace Timberborn.Carrying;

public class CarrierInventoryFinder : BaseComponent, IAwakableComponent
{
	private readonly CarryAmountCalculator _carryAmountCalculator;

	private GoodCarrier _goodCarrier;

	private GoodReserver _goodReserver;

	private Navigator _navigator;

	private Citizen _citizen;

	public CarrierInventoryFinder(CarryAmountCalculator carryAmountCalculator)
	{
		_carryAmountCalculator = carryAmountCalculator;
	}

	public void Awake()
	{
		_goodCarrier = GetComponent<GoodCarrier>();
		_goodReserver = GetComponent<GoodReserver>();
		_navigator = GetComponent<Navigator>();
		_citizen = GetComponent<Citizen>();
	}

	public bool TryCarryFromAnyInventory(string goodId, Inventory receivingInventory)
	{
		return TryCarryFromAnyInventoryInternal(goodId, receivingInventory, (Inventory _) => true);
	}

	public bool TryCarryFromAnyInventory(string goodId, Inventory receivingInventory, Predicate<Inventory> inventoryFilter)
	{
		return TryCarryFromAnyInventoryInternal(goodId, receivingInventory, inventoryFilter);
	}

	public bool TryCarryFromAnyInventoryLimited(string goodId, Inventory receivingInventory, int maxAmount)
	{
		return TryCarryFromAnyInventoryInternal(goodId, receivingInventory, (Inventory _) => true, maxAmount);
	}

	public bool TryCarryToAnyInventory(string goodId, Inventory givingInventory, Predicate<Inventory> inventoryFilter)
	{
		Accessible enabledComponent = givingInventory.GetEnabledComponent<Accessible>();
		DistrictCenter district = givingInventory.GetComponent<DistrictBuilding>().District;
		if ((bool)district)
		{
			GoodAmount goodAmount = new GoodAmount(goodId, 1);
			Inventory inventory = district.GetComponent<DistrictInventoryPicker>().ClosestInventoryWithCapacity(enabledComponent, goodAmount, inventoryFilter, out var _);
			if (inventory != null)
			{
				GoodAmount goodAmount2 = _carryAmountCalculator.AmountToCarry(_goodCarrier.LiftingCapacity, goodId, inventory, givingInventory);
				if (goodAmount2.Amount > 0)
				{
					_goodReserver.ReserveNotLessThanStockAmount(givingInventory, goodAmount2);
					_goodReserver.ReserveCapacity(inventory, goodAmount2);
					return true;
				}
			}
		}
		return false;
	}

	public Inventory GetClosestInventoryWithCapacity(string goodId, Accessible accessible, out float distance)
	{
		GoodAmount goodAmount = new GoodAmount(goodId, 1);
		distance = float.MaxValue;
		DistrictCenter district = GetDistrict(accessible);
		if (district != null)
		{
			return district.GetComponent<DistrictInventoryPicker>().ClosestInventoryWithCapacity(accessible, goodAmount, (Inventory _) => true, out distance);
		}
		if (_citizen.HasAssignedDistrict)
		{
			DistrictInventoryPicker component = _citizen.AssignedDistrict.GetComponent<DistrictInventoryPicker>();
			return component.ClosestInventoryWithCapacity(accessible.Transform.position, goodAmount, out distance) ?? component.ClosestInventoryWithCapacity(_navigator.CurrentAccessOrPosition(), goodAmount, out distance);
		}
		return null;
	}

	private bool TryCarryFromAnyInventoryInternal(string goodId, Inventory receivingInventory, Predicate<Inventory> inventoryFilter, int? maxAmount = null)
	{
		Accessible enabledComponent = receivingInventory.GetEnabledComponent<Accessible>();
		DistrictCenter district = receivingInventory.GetComponent<DistrictBuilding>().District;
		if ((bool)district)
		{
			Inventory inventory = district.GetComponent<DistrictInventoryPicker>().ClosestInventoryWithStock(enabledComponent, goodId, inventoryFilter);
			if (inventory != null && TryReserveInventories(goodId, receivingInventory, inventory, maxAmount))
			{
				return true;
			}
		}
		return false;
	}

	private bool TryReserveInventories(string goodId, Inventory receivingInventory, Inventory givingInventory, int? maxAmount)
	{
		GoodAmount carriableGood = GetCarriableGood(goodId, receivingInventory, givingInventory, maxAmount);
		if (carriableGood.Amount > 0)
		{
			if (maxAmount.HasValue)
			{
				_goodReserver.ReserveExactStockAmount(givingInventory, carriableGood);
			}
			else
			{
				_goodReserver.ReserveNotLessThanStockAmount(givingInventory, carriableGood);
			}
			_goodReserver.ReserveCapacity(receivingInventory, carriableGood);
			return true;
		}
		return false;
	}

	private GoodAmount GetCarriableGood(string goodId, Inventory receivingInventory, Inventory givingInventory, int? maxAmount)
	{
		GoodAmount result = _carryAmountCalculator.AmountToCarry(_goodCarrier.LiftingCapacity, goodId, receivingInventory, givingInventory);
		if (maxAmount.HasValue && result.Amount > maxAmount)
		{
			return new GoodAmount(goodId, maxAmount.Value);
		}
		return result;
	}

	private static DistrictCenter GetDistrict(Accessible accessible)
	{
		DistrictBuilding component = accessible.GetComponent<DistrictBuilding>();
		if (!component)
		{
			return null;
		}
		return component.GetDistrictOrConstructionDistrict();
	}
}
