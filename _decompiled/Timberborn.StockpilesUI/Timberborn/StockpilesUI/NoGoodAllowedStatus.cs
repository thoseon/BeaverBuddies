using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.InventorySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.StockpilesUI;

internal class NoGoodAllowedStatus : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private static readonly string NoGoodSelectedLocKey = "Status.Inventory.NoGoodSelected";

	private static readonly string NoGoodSelectedShortLocKey = "Status.Inventory.NoGoodSelected.Short";

	private readonly ILoc _loc;

	private SingleGoodAllower _singleGoodAllower;

	private StatusToggle _noGoodSelectedStatusToggle;

	public NoGoodAllowedStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_singleGoodAllower = GetComponent<SingleGoodAllower>();
		_noGoodSelectedStatusToggle = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon("UnspecifiedGood", _loc.T(NoGoodSelectedLocKey), _loc.T(NoGoodSelectedShortLocKey));
		DisableComponent();
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_noGoodSelectedStatusToggle);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		UpdateStatusToggle();
		_singleGoodAllower.DisallowedGoodsChanged += OnDisallowedGoodsChanged;
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		UpdateStatusToggle();
		_singleGoodAllower.DisallowedGoodsChanged -= OnDisallowedGoodsChanged;
	}

	private void OnDisallowedGoodsChanged(object sender, DisallowedGoodsChangedEventArgs e)
	{
		UpdateStatusToggle();
	}

	private void UpdateStatusToggle()
	{
		if (base.Enabled && !_singleGoodAllower.HasAllowedGood)
		{
			_noGoodSelectedStatusToggle.Activate();
		}
		else
		{
			_noGoodSelectedStatusToggle.Deactivate();
		}
	}
}
