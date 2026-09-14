using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;

namespace Timberborn.GoodStackSystem;

public class GoodStack : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity, IGoodStackInventory
{
	private readonly GoodStackModelFactory _goodStackModelFactory;

	private GoodStackAccessible _goodStackAccessible;

	private GoodStackModel _goodStackModel;

	public Inventory Inventory { get; private set; }

	public event EventHandler GoodStackDisabled;

	public GoodStack(GoodStackModelFactory goodStackModelFactory)
	{
		_goodStackModelFactory = goodStackModelFactory;
	}

	public void Awake()
	{
		_goodStackAccessible = GetComponent<GoodStackAccessible>();
		_goodStackModel = GetComponent<GoodStackModel>();
		DisableComponent();
	}

	public void InitializeInventory(Inventory inventory)
	{
		Asserts.FieldIsNull(this, Inventory, "Inventory");
		Inventory = inventory;
	}

	public void InitializeEntity()
	{
		_goodStackModelFactory.Create(this);
		if (!Inventory.IsEmpty)
		{
			EnableGoodStack();
			_goodStackModel.UpdateModel(Inventory);
		}
	}

	public void DeleteEntity()
	{
		DisableGoodStack();
	}

	public void EnableGoodStack(GoodAmount goodAmount)
	{
		EnableGoodStack();
		Inventory.GiveProduced(goodAmount);
		_goodStackModel.UpdateModel(Inventory);
	}

	private void EnableGoodStack()
	{
		Inventory.Enable();
		Inventory.InventoryChanged += UpdateGoodStack;
		_goodStackAccessible.Enable();
		EnableComponent();
	}

	private void DisableGoodStack()
	{
		Inventory.Disable();
		Inventory.InventoryChanged -= UpdateGoodStack;
		_goodStackAccessible.Disable();
		GoodStackDisabled?.Invoke(this, EventArgs.Empty);
		DisableComponent();
	}

	private void UpdateGoodStack(object sender, InventoryChangedEventArgs e)
	{
		_goodStackModel.UpdateModel(Inventory);
		if (Inventory.IsEmpty)
		{
			DisableGoodStack();
		}
	}
}
