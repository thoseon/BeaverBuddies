using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.GameDistricts;
using Timberborn.InventorySystem;

namespace Timberborn.Emptying;

internal class DistrictEmptiableInventoriesRegistry : BaseComponent, IAwakableComponent
{
	private readonly List<Inventories> _emptiableInventories = new List<Inventories>();

	public ReadOnlyList<Inventories> EmptiableInventories => _emptiableInventories.AsReadOnlyList();

	public void Awake()
	{
		DistrictBuildingRegistry component = GetComponent<DistrictBuildingRegistry>();
		component.FinishedBuildingRegistered += OnFinishedBuildingRegistered;
		component.FinishedBuildingUnregistered += OnFinishedBuildingUnregistered;
	}

	private void OnFinishedBuildingRegistered(object sender, FinishedBuildingRegisteredEventArgs e)
	{
		Emptiable component = e.Building.GetComponent<Emptiable>();
		if (component != null)
		{
			component.MarkedForEmptying += OnMarkedForEmptying;
			component.UnmarkedForEmptying += OnUnmarkedForEmptying;
			if (component.IsMarkedForEmptying)
			{
				Add(component);
			}
		}
	}

	private void OnFinishedBuildingUnregistered(object sender, FinishedBuildingUnregisteredEventArgs e)
	{
		Emptiable component = e.Building.GetComponent<Emptiable>();
		if (component != null)
		{
			component.MarkedForEmptying -= OnMarkedForEmptying;
			component.UnmarkedForEmptying -= OnUnmarkedForEmptying;
			Remove(component);
		}
	}

	private void OnMarkedForEmptying(object sender, EventArgs e)
	{
		Add((Emptiable)sender);
	}

	private void OnUnmarkedForEmptying(object sender, EventArgs e)
	{
		Remove((Emptiable)sender);
	}

	private void Add(Emptiable emptiable)
	{
		Inventories component = emptiable.GetComponent<Inventories>();
		if (_emptiableInventories.Contains(component))
		{
			return;
		}
		_emptiableInventories.Add(component);
		foreach (Inventory allInventory in component.AllInventories)
		{
			allInventory.SetIgnorableCapacity(ignorableCapacity: true);
		}
	}

	private void Remove(Emptiable emptiable)
	{
		Inventories component = emptiable.GetComponent<Inventories>();
		if (!_emptiableInventories.Remove(component))
		{
			return;
		}
		foreach (Inventory allInventory in component.AllInventories)
		{
			allInventory.SetIgnorableCapacity(ignorableCapacity: false);
		}
	}
}
