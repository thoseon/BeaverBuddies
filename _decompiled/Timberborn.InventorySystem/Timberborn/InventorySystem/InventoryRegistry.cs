using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Goods;

namespace Timberborn.InventorySystem;

internal class InventoryRegistry
{
	private readonly Dictionary<string, HashSet<Inventory>> _inventoriesWithCapacity = new Dictionary<string, HashSet<Inventory>>();

	private readonly Dictionary<string, HashSet<Inventory>> _inventoriesWithStock = new Dictionary<string, HashSet<Inventory>>();

	private readonly HashSet<Inventory> _inventories = new HashSet<Inventory>();

	public ReadOnlyHashSet<Inventory> Inventories => _inventories.AsReadOnlyHashSet();

	public InventoryRegistry(ReadOnlyList<string> goods)
	{
		foreach (string item in goods)
		{
			_inventoriesWithCapacity[item] = new HashSet<Inventory>();
			_inventoriesWithStock[item] = new HashSet<Inventory>();
		}
	}

	public ReadOnlyHashSet<Inventory> InventoriesWithCapacity(string goodId)
	{
		return _inventoriesWithCapacity[goodId].AsReadOnlyHashSet();
	}

	public ReadOnlyHashSet<Inventory> InventoriesWithStock(string goodId)
	{
		return _inventoriesWithStock[goodId].AsReadOnlyHashSet();
	}

	public void RegisterInventory(Inventory inventory)
	{
		_inventories.Add(inventory);
		string key;
		HashSet<Inventory> value;
		if (inventory.PublicInput)
		{
			foreach (KeyValuePair<string, HashSet<Inventory>> item in _inventoriesWithCapacity)
			{
				item.Deconstruct(out key, out value);
				string goodId = key;
				HashSet<Inventory> hashSet = value;
				if (inventory.HasUnreservedCapacity(goodId))
				{
					hashSet.Add(inventory);
				}
			}
		}
		if (inventory.PublicOutput)
		{
			foreach (KeyValuePair<string, HashSet<Inventory>> item2 in _inventoriesWithStock)
			{
				item2.Deconstruct(out key, out value);
				string goodId2 = key;
				HashSet<Inventory> hashSet2 = value;
				if (inventory.HasUnreservedStock(goodId2))
				{
					hashSet2.Add(inventory);
				}
			}
		}
		if (inventory.PublicInput || inventory.PublicOutput)
		{
			inventory.InventoryChanged += OnInventoryChanged;
			inventory.UnwantedStockDisappeared += OnUnwantedStockDisappeared;
		}
	}

	public void UnregisterInventory(Inventory inventory)
	{
		_inventories.Remove(inventory);
		if (inventory.PublicInput)
		{
			foreach (HashSet<Inventory> value in _inventoriesWithCapacity.Values)
			{
				value.Remove(inventory);
			}
		}
		if (inventory.PublicOutput)
		{
			foreach (HashSet<Inventory> value2 in _inventoriesWithStock.Values)
			{
				value2.Remove(inventory);
			}
		}
		if (inventory.PublicInput || inventory.PublicOutput)
		{
			inventory.InventoryChanged -= OnInventoryChanged;
			inventory.UnwantedStockDisappeared -= OnUnwantedStockDisappeared;
		}
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		UpdateRegistries((Inventory)sender, e.GoodId);
	}

	private void OnUnwantedStockDisappeared(object sender, EventArgs e)
	{
		Inventory inventory = (Inventory)sender;
		foreach (StorableGoodAmount allowedGood in inventory.AllowedGoods)
		{
			UpdateRegistries(inventory, allowedGood.StorableGood.GoodId);
		}
	}

	private void UpdateRegistries(Inventory inventory, string good)
	{
		if (inventory.PublicInput)
		{
			HashSet<Inventory> hashSet = _inventoriesWithCapacity[good];
			if (inventory.HasUnreservedCapacity(good))
			{
				hashSet.Add(inventory);
			}
			else
			{
				hashSet.Remove(inventory);
			}
		}
		if (inventory.PublicOutput)
		{
			HashSet<Inventory> hashSet2 = _inventoriesWithStock[good];
			if (inventory.HasUnreservedStock(good))
			{
				hashSet2.Add(inventory);
			}
			else
			{
				hashSet2.Remove(inventory);
			}
		}
	}
}
