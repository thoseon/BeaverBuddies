using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.GameDistricts;

namespace Timberborn.InventorySystem;

internal class DistrictInventoryAssigner : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private DistrictBuilding _districtBuilding;

	private DistrictInventoryRegistry _districtInventoryRegistry;

	private readonly List<Inventory> _inventories = new List<Inventory>();

	public void Awake()
	{
		_districtBuilding = GetComponent<DistrictBuilding>();
	}

	public void StartAssigningInventory(Inventory inventory)
	{
		if ((bool)_districtInventoryRegistry)
		{
			_districtInventoryRegistry.Add(inventory);
		}
		_inventories.Add(inventory);
	}

	public void StopAssigningInventory(Inventory inventory)
	{
		if ((bool)_districtInventoryRegistry)
		{
			_districtInventoryRegistry.Remove(inventory);
		}
		_inventories.Remove(inventory);
	}

	public void OnEnterFinishedState()
	{
		AddRegisteredInventories();
		_districtBuilding.ReassignedDistrict += OnReassignedDistrict;
	}

	public void OnExitFinishedState()
	{
		RemoveRegisteredInventories();
		_districtBuilding.ReassignedDistrict -= OnReassignedDistrict;
	}

	private void OnReassignedDistrict(object sender, EventArgs e)
	{
		RemoveRegisteredInventories();
		AddRegisteredInventories();
	}

	private void RemoveRegisteredInventories()
	{
		if (!_districtInventoryRegistry)
		{
			return;
		}
		foreach (Inventory inventory in _inventories)
		{
			_districtInventoryRegistry.Remove(inventory);
		}
		_districtInventoryRegistry = null;
	}

	private void AddRegisteredInventories()
	{
		DistrictCenter district = _districtBuilding.District;
		if (!district)
		{
			return;
		}
		_districtInventoryRegistry = district.GetComponent<DistrictInventoryRegistry>();
		foreach (Inventory inventory in _inventories)
		{
			_districtInventoryRegistry.Add(inventory);
		}
	}
}
