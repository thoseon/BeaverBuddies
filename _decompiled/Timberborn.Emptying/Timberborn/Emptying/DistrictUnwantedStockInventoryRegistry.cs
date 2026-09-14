using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.InventorySystem;

namespace Timberborn.Emptying;

internal class DistrictUnwantedStockInventoryRegistry : BaseComponent, IAwakableComponent
{
	private readonly List<Inventory> _inventoriesWithUnwantedStock = new List<Inventory>();

	public ReadOnlyList<Inventory> InventoriesWithUnwantedStock => _inventoriesWithUnwantedStock.AsReadOnlyList();

	public void Awake()
	{
		DistrictInventoryRegistry component = GetComponent<DistrictInventoryRegistry>();
		component.InventoryRegistered += OnInventoryRegistered;
		component.InventoryUnregistered += OnInventoryUnregistered;
	}

	private void OnInventoryRegistered(object sender, Inventory inventory)
	{
		inventory.InventoryChanged += OnInventoryChanged;
		if (inventory.HasUnwantedStock)
		{
			Add(inventory);
		}
	}

	private void OnInventoryUnregistered(object sender, Inventory inventory)
	{
		inventory.InventoryChanged -= OnInventoryChanged;
		Remove(inventory);
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		UpdateState((Inventory)sender);
	}

	private void UpdateState(Inventory inventory)
	{
		if (inventory.HasUnwantedStock)
		{
			Add(inventory);
		}
		else
		{
			Remove(inventory);
		}
	}

	private void Add(Inventory inventory)
	{
		if (!_inventoriesWithUnwantedStock.Contains(inventory))
		{
			_inventoriesWithUnwantedStock.Add(inventory);
		}
	}

	private void Remove(Inventory inventory)
	{
		_inventoriesWithUnwantedStock.Remove(inventory);
	}
}
