using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;

namespace Timberborn.InventorySystem;

public class Inventories : BaseComponent
{
	private readonly List<Inventory> _inventories = new List<Inventory>();

	private readonly List<Inventory> _enabledInventories = new List<Inventory>();

	public ReadOnlyList<Inventory> AllInventories => _inventories.AsReadOnlyList();

	public ReadOnlyList<Inventory> EnabledInventories => _enabledInventories.AsReadOnlyList();

	public void AddInventory(Inventory inventory)
	{
		_inventories.Add(inventory);
		if (inventory.Enabled)
		{
			_enabledInventories.Add(inventory);
		}
		inventory.InventoryEnabled += delegate
		{
			_enabledInventories.Add(inventory);
		};
		inventory.InventoryDisabled += delegate
		{
			_enabledInventories.Remove(inventory);
		};
	}
}
