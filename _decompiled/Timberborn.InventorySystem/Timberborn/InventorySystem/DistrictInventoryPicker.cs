using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockingSystem;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.InventorySystem;

public class DistrictInventoryPicker : BaseComponent, IAwakableComponent
{
	private DistrictInventoryRegistry _districtInventoryRegistry;

	public void Awake()
	{
		_districtInventoryRegistry = GetComponent<DistrictInventoryRegistry>();
	}

	public Inventory ClosestInventoryWithStock(Accessible start, string goodId, Predicate<Inventory> inventoryFilter)
	{
		ReadOnlyHashSet<Inventory> readOnlyHashSet = _districtInventoryRegistry.ActiveInventoriesWithStock(goodId);
		Inventory result = null;
		float num = float.MaxValue;
		foreach (Inventory item in readOnlyHashSet)
		{
			Accessible enabledComponent = item.GetEnabledComponent<Accessible>();
			if (inventoryFilter(item) && start.FindRoadPath(enabledComponent, out var distance) && distance < num)
			{
				result = item;
				num = distance;
			}
		}
		return result;
	}

	public Inventory ClosestInventoryWithStock(Vector3 start, string goodId, Accessible accessibleReachableFromInventory)
	{
		ReadOnlyHashSet<Inventory> readOnlyHashSet = _districtInventoryRegistry.ActiveInventoriesWithStock(goodId);
		Inventory result = null;
		float num = float.MaxValue;
		foreach (Inventory item in readOnlyHashSet)
		{
			Accessible enabledComponent = item.GetEnabledComponent<Accessible>();
			if (enabledComponent.IsReachableByRoad(accessibleReachableFromInventory) && enabledComponent.FindRoadPath(start, out var distance) && distance < num)
			{
				result = item;
				num = distance;
			}
		}
		return result;
	}

	public Inventory ClosestInventoryWithCapacity(Vector3 start, GoodAmount goodAmount, out float closestDistance)
	{
		ReadOnlyHashSet<Inventory> inventories = _districtInventoryRegistry.ActiveInventoriesWithCapacity(goodAmount.GoodId);
		Inventory inventory = null;
		closestDistance = float.MaxValue;
		foreach (Inventory item in inventories)
		{
			Accessible enabledComponent = item.GetEnabledComponent<Accessible>();
			if ((enabledComponent.FindRoadPath(start, out var distance) || enabledComponent.FindRoadToTerrainPath(start, out distance)) && distance < closestDistance && InventoryIsTaking(item, goodAmount))
			{
				inventory = item;
				closestDistance = distance;
			}
		}
		if (!inventory)
		{
			return ClosestInventoryInStraightLine(start, goodAmount, inventories, out closestDistance);
		}
		return inventory;
	}

	public Inventory ClosestInventoryWithCapacity(Accessible start, GoodAmount goodAmount, Predicate<Inventory> inventoryFilter, out float closestDistance)
	{
		ReadOnlyHashSet<Inventory> readOnlyHashSet = _districtInventoryRegistry.ActiveInventoriesWithCapacity(goodAmount.GoodId);
		Inventory result = null;
		closestDistance = float.MaxValue;
		foreach (Inventory item in readOnlyHashSet)
		{
			Accessible enabledComponent = item.GetEnabledComponent<Accessible>();
			if (inventoryFilter(item) && start.FindRoadPath(enabledComponent, out var distance) && distance < closestDistance && InventoryIsTaking(item, goodAmount))
			{
				result = item;
				closestDistance = distance;
			}
		}
		return result;
	}

	private static bool InventoryIsTaking(Inventory inventory, GoodAmount goodAmount)
	{
		if (inventory.HasUnreservedCapacity(goodAmount) && inventory.GetComponent<IInventoryValidator>().ValidInventory)
		{
			return inventory.GetComponent<BlockableObject>().IsUnblocked;
		}
		return false;
	}

	private static Inventory ClosestInventoryInStraightLine(Vector3 start, GoodAmount goodAmount, ReadOnlyHashSet<Inventory> inventories, out float closestDistance)
	{
		Inventory inventory = null;
		closestDistance = float.MaxValue;
		foreach (Inventory item in inventories)
		{
			float num = Vector3.Distance(start, item.Transform.position);
			if (num < closestDistance && InventoryIsTaking(item, goodAmount))
			{
				inventory = item;
				closestDistance = num;
			}
		}
		if (!inventory || !inventory.GetEnabledComponent<Accessible>().IsReachableUnlimitedRange(start))
		{
			return null;
		}
		return inventory;
	}
}
