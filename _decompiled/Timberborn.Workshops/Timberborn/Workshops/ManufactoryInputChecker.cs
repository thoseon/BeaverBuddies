using System;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.WorkSystem;

namespace Timberborn.Workshops;

internal class ManufactoryInputChecker : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private Manufactory _manufactory;

	private Workplace _workplace;

	private DistrictBuilding _districtBuilding;

	private LackOfResourcesStatus _lackOfResourcesStatus;

	private bool _inputUnavailable;

	public void Awake()
	{
		_manufactory = GetComponent<Manufactory>();
		_workplace = GetComponent<Workplace>();
		_districtBuilding = GetComponent<DistrictBuilding>();
		_lackOfResourcesStatus = GetComponent<LackOfResourcesStatus>();
	}

	public void OnEnterFinishedState()
	{
		_lackOfResourcesStatus.Initialize(() => _inputUnavailable);
		CheckIfInputIsUnavailable();
		if (_manufactory.NeedsInventory)
		{
			_manufactory.Inventory.InventoryChanged += OnInventoryChanged;
			if ((bool)_workplace)
			{
				_workplace.WorkerAssigned += OnWorkerChanged;
				_workplace.WorkerUnassigned += OnWorkerChanged;
			}
			if ((bool)_districtBuilding)
			{
				_districtBuilding.ReassignedDistrict += OnDistrictReassigned;
			}
		}
	}

	public void OnExitFinishedState()
	{
		_lackOfResourcesStatus.Disable();
		if (_manufactory.NeedsInventory)
		{
			_manufactory.Inventory.InventoryChanged -= OnInventoryChanged;
			if ((bool)_workplace)
			{
				_workplace.WorkerAssigned -= OnWorkerChanged;
				_workplace.WorkerUnassigned -= OnWorkerChanged;
			}
			if ((bool)_districtBuilding)
			{
				_districtBuilding.ReassignedDistrict -= OnDistrictReassigned;
			}
		}
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		CheckIfInputIsUnavailable();
	}

	private void OnWorkerChanged(object sender, WorkerChangedEventArgs e)
	{
		CheckIfInputIsUnavailable();
	}

	private void OnDistrictReassigned(object sender, EventArgs e)
	{
		CheckIfInputIsUnavailable();
	}

	private void CheckIfInputIsUnavailable()
	{
		if (!_workplace || _workplace.NumberOfAssignedWorkers == 0 || !_manufactory.HasCurrentRecipe || !_districtBuilding.District)
		{
			_inputUnavailable = false;
		}
		else
		{
			_inputUnavailable = InputIsUnavailable();
		}
	}

	private bool InputIsUnavailable()
	{
		DistrictInventoryRegistry component = _districtBuilding.District.GetComponent<DistrictInventoryRegistry>();
		if (!FuelIsUnavailable(component))
		{
			return IngredientsAreUnavailable(component);
		}
		return true;
	}

	private bool FuelIsUnavailable(DistrictInventoryRegistry inventoryRegistry)
	{
		string fuel = _manufactory.CurrentRecipe.Fuel;
		if (!_manufactory.HasFuel)
		{
			return inventoryRegistry.ActiveInventoriesWithStock(fuel).Count == 0;
		}
		return false;
	}

	private bool IngredientsAreUnavailable(DistrictInventoryRegistry inventoryRegistry)
	{
		if (!_manufactory.HasAllIngredients)
		{
			ImmutableArray<GoodAmountSpec> ingredients = _manufactory.CurrentRecipe.Ingredients;
			for (int i = 0; i < ingredients.Length; i++)
			{
				if (inventoryRegistry.ActiveInventoriesWithStock(ingredients[i].Id).Count == 0)
				{
					return true;
				}
			}
		}
		return false;
	}
}
