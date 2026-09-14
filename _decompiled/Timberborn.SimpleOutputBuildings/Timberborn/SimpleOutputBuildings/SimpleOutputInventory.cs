using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.InventorySystem;

namespace Timberborn.SimpleOutputBuildings;

public class SimpleOutputInventory : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	public Inventory Inventory { get; private set; }

	public void Awake()
	{
		DisableComponent();
	}

	public void InitializeInventory(Inventory inventory)
	{
		Asserts.FieldIsNull(this, Inventory, "Inventory");
		Inventory = inventory;
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		Inventory.Enable();
	}

	public void OnExitFinishedState()
	{
		Inventory.Disable();
		DisableComponent();
	}
}
