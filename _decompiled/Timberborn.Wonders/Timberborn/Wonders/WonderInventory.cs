using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Localization;
using Timberborn.ResourceCountingSystem;
using Timberborn.StatusSystem;

namespace Timberborn.Wonders;

public class WonderInventory : BaseComponent, IAwakableComponent, IInitializableEntity, IWonderBlocker, IFinishedStateListener, IGoodProcessor
{
	private static readonly string DisallowReasonLocKey = "Status.Wonder.NotEnoughGoods";

	private readonly ILoc _loc;

	private Wonder _wonder;

	private WonderInventorySpec _wonderInventorySpec;

	private StatusToggle _statusToggle;

	public Inventory Inventory { get; private set; }

	public ReadOnlyList<GoodAmount> ProcessedGoods { get; } = new List<GoodAmount>().AsReadOnlyList();

	public ImmutableArray<GoodAmountSpec> RequiredGoods => _wonderInventorySpec.RequiredGoods;

	public WonderInventory(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_wonder = GetComponent<Wonder>();
		_wonderInventorySpec = GetComponent<WonderInventorySpec>();
		_statusToggle = StatusToggle.CreateNormalStatus("LackOfResources", _loc.T(DisallowReasonLocKey));
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
	}

	public void OnEnterFinishedState()
	{
		Inventory.Enable();
		Inventory.InventoryChanged += delegate
		{
			UpdateStatus();
		};
		_wonder.WonderActivated += OnWonderActivated;
		UpdateStatus();
	}

	public void OnExitFinishedState()
	{
		Inventory.Disable();
	}

	public bool IsWonderBlocked()
	{
		return !Inventory.IsFull;
	}

	public void InitializeInventory(Inventory inventory)
	{
		Asserts.FieldIsNull(this, Inventory, "Inventory");
		Inventory = inventory;
	}

	private void OnWonderActivated(object sender, EventArgs e)
	{
		ClearInventory();
	}

	private void ClearInventory()
	{
		foreach (GoodAmountSpec requiredGood in RequiredGoods)
		{
			Inventory.TakeConsumed(new GoodAmount(requiredGood.Id, requiredGood.Amount));
		}
	}

	private void UpdateStatus()
	{
		if (!IsWonderBlocked() || _wonder.IsActive)
		{
			_statusToggle.Deactivate();
		}
		else
		{
			_statusToggle.Activate();
		}
	}
}
