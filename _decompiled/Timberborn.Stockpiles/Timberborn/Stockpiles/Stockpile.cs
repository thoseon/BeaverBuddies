using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.InventorySystem;

namespace Timberborn.Stockpiles;

public class Stockpile : BaseComponent, IAwakableComponent, IRegisteredComponent, IFinishedStateListener
{
	private StockpileSpec _stockpileSpec;

	public Inventory Inventory { get; private set; }

	public int MaxCapacity => _stockpileSpec.MaxCapacity;

	public string WhitelistedGoodType => _stockpileSpec.WhitelistedGoodType;

	public void Awake()
	{
		_stockpileSpec = GetComponent<StockpileSpec>();
		DisableComponent();
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

	public void InitializeInventory(Inventory inventory)
	{
		Asserts.FieldIsNull(this, Inventory, "Inventory");
		Inventory = inventory;
	}
}
