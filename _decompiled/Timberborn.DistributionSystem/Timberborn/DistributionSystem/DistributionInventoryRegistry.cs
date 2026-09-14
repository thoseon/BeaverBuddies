using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.StockpilePrioritySystem;
using Timberborn.Stockpiles;

namespace Timberborn.DistributionSystem;

internal class DistributionInventoryRegistry : BaseComponent, IAwakableComponent
{
	private readonly IGoodService _goodService;

	private readonly Dictionary<string, HashSet<Inventory>> _storingInventories = new Dictionary<string, HashSet<Inventory>>();

	private readonly Dictionary<string, HashSet<Inventory>> _stockInventories = new Dictionary<string, HashSet<Inventory>>();

	private readonly Dictionary<string, HashSet<Inventory>> _capacityInventories = new Dictionary<string, HashSet<Inventory>>();

	private readonly HashSet<Inventory> _districtCrossingInventories = new HashSet<Inventory>();

	public ReadOnlyHashSet<Inventory> DistrictCrossingInventories => _districtCrossingInventories.AsReadOnlyHashSet();

	public event EventHandler<string> GoodStorageChanged;

	public DistributionInventoryRegistry(IGoodService goodService)
	{
		_goodService = goodService;
	}

	public ReadOnlyHashSet<Inventory> StoringInventories(string goodId)
	{
		return _storingInventories[goodId].AsReadOnlyHashSet();
	}

	public ReadOnlyHashSet<Inventory> StockInventories(string goodId)
	{
		return _stockInventories[goodId].AsReadOnlyHashSet();
	}

	public ReadOnlyHashSet<Inventory> CapacityInventories(string goodId)
	{
		return _capacityInventories[goodId].AsReadOnlyHashSet();
	}

	public void Awake()
	{
		DistrictInventoryRegistry component = GetComponent<DistrictInventoryRegistry>();
		component.InventoryRegistered += OnInventoryRegistered;
		component.InventoryUnregistered += OnInventoryUnregistered;
		InitializeRegistries();
	}

	private void InitializeRegistries()
	{
		foreach (string good in _goodService.Goods)
		{
			_storingInventories[good] = new HashSet<Inventory>();
			_stockInventories[good] = new HashSet<Inventory>();
			_capacityInventories[good] = new HashSet<Inventory>();
		}
	}

	private void OnInventoryRegistered(object sender, Inventory inventory)
	{
		UpdateRegistries(inventory);
		if (inventory.ComponentName == DistrictCrossingInventoryInitializer.InventoryComponentName)
		{
			_districtCrossingInventories.Add(inventory);
		}
		inventory.InventoryChanged += OnInventoryChanged;
		inventory.UnwantedStockDisappeared += OnUnwantedStockDisappeared;
		StockpilePriorityChangeListener component = inventory.GetComponent<StockpilePriorityChangeListener>();
		if (component != null)
		{
			component.PriorityChanged += OnPriorityChanged;
		}
	}

	private void OnInventoryUnregistered(object sender, Inventory inventory)
	{
		foreach (StorableGoodAmount allowedGood in inventory.AllowedGoods)
		{
			RemoveInventory(inventory, allowedGood.StorableGood.GoodId);
		}
		_districtCrossingInventories.Remove(inventory);
		inventory.InventoryChanged -= OnInventoryChanged;
		inventory.UnwantedStockDisappeared -= OnUnwantedStockDisappeared;
		StockpilePriorityChangeListener component = inventory.GetComponent<StockpilePriorityChangeListener>();
		if (component != null)
		{
			component.PriorityChanged -= OnPriorityChanged;
		}
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		UpdateInventory((Inventory)sender, e.GoodId);
	}

	private void OnUnwantedStockDisappeared(object sender, EventArgs e)
	{
		UpdateRegistries((Inventory)sender);
	}

	private void OnPriorityChanged(object sender, EventArgs e)
	{
		Stockpile component = ((StockpilePriorityChangeListener)sender).GetComponent<Stockpile>();
		UpdateRegistries(component.Inventory);
	}

	private void UpdateRegistries(Inventory inventory)
	{
		foreach (StorableGoodAmount allowedGood in inventory.AllowedGoods)
		{
			UpdateInventory(inventory, allowedGood.StorableGood.GoodId);
		}
	}

	private void UpdateInventory(Inventory inventory, string goodId)
	{
		if (IsStoringInventory(inventory, goodId))
		{
			_storingInventories[goodId].Add(inventory);
		}
		else
		{
			_storingInventories[goodId].Remove(inventory);
		}
		if (IsStockInventory(inventory, goodId))
		{
			_stockInventories[goodId].Add(inventory);
		}
		else
		{
			_stockInventories[goodId].Remove(inventory);
		}
		if (IsCapacityInventory(inventory, goodId))
		{
			_capacityInventories[goodId].Add(inventory);
		}
		else
		{
			_capacityInventories[goodId].Remove(inventory);
		}
		GoodStorageChanged?.Invoke(this, goodId);
	}

	private void RemoveInventory(Inventory inventory, string goodId)
	{
		_storingInventories[goodId].Remove(inventory);
		_stockInventories[goodId].Remove(inventory);
		_capacityInventories[goodId].Remove(inventory);
		GoodStorageChanged?.Invoke(this, goodId);
	}

	private static bool IsStoringInventory(Inventory inventory, string goodId)
	{
		if (inventory.Gives(goodId) && inventory.Takes(goodId))
		{
			return inventory.LimitedAmount(goodId) > 0;
		}
		return false;
	}

	private static bool IsStockInventory(Inventory inventory, string goodId)
	{
		return inventory.Gives(goodId);
	}

	private static bool IsCapacityInventory(Inventory inventory, string goodId)
	{
		if (inventory.Takes(goodId))
		{
			return inventory.LimitedAmount(goodId) > 0;
		}
		return false;
	}
}
