using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.InventorySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;
using Timberborn.Stockpiles;

namespace Timberborn.StockpilesUI;

internal class UnwantedStockStatus : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private static readonly string UnwantedStockLocKey = "Status.Inventory.UnwantedStock";

	private readonly ILoc _loc;

	private SingleGoodAllower _singleGoodAllower;

	private Inventory _inventory;

	private StatusToggle _unwantedStockStatusToggle;

	public UnwantedStockStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_singleGoodAllower = GetComponent<SingleGoodAllower>();
		_inventory = GetComponent<Stockpile>().Inventory;
		_unwantedStockStatusToggle = StatusToggle.CreateNormalStatusWithFloatingIcon("Empty", _loc.T(UnwantedStockLocKey));
		DisableComponent();
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_unwantedStockStatusToggle);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		UpdateStatusToggle();
		_singleGoodAllower.DisallowedGoodsChanged += OnDisallowedGoodsChanged;
		_inventory.InventoryChanged += OnInventoryChanged;
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		UpdateStatusToggle();
		_singleGoodAllower.DisallowedGoodsChanged -= OnDisallowedGoodsChanged;
		_inventory.InventoryChanged -= OnInventoryChanged;
	}

	private void OnDisallowedGoodsChanged(object sender, DisallowedGoodsChangedEventArgs e)
	{
		UpdateStatusToggle();
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		UpdateStatusToggle();
	}

	private void UpdateStatusToggle()
	{
		if (base.Enabled && _inventory.HasUnwantedStock)
		{
			_unwantedStockStatusToggle.Activate();
		}
		else
		{
			_unwantedStockStatusToggle.Deactivate();
		}
	}
}
