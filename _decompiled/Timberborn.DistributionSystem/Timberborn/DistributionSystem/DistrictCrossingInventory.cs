using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.LinkedBuildingSystem;

namespace Timberborn.DistributionSystem;

public class DistrictCrossingInventory : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private LinkedInventories _linkedInventories;

	private DistrictCrossingInventory _linked;

	private readonly MirrorOperationLock _mirrorOperationLock = new MirrorOperationLock();

	public Inventory Inventory { get; private set; }

	public void Awake()
	{
		_linkedInventories = GetComponent<LinkedInventories>();
		GetComponent<LinkedBuilding>().BuildingLinked += OnBuildingLinked;
	}

	public void InitializeEntity()
	{
		if ((bool)_linked)
		{
			ReserveStock();
		}
	}

	public void InitializeInventory(Inventory inventory)
	{
		Asserts.FieldIsNull(this, Inventory, "Inventory");
		Inventory = inventory;
	}

	public void TransferStock(string goodId, int amount)
	{
		if (_mirrorOperationLock.IsUnlocked)
		{
			amount = Math.Min(amount, Inventory.UnreservedAmountInStock(goodId));
			if (amount > 0)
			{
				GoodAmount goodAmount = new GoodAmount(goodId, amount);
				Inventory.TakeExported(goodAmount);
				_linked.GiveStock(goodAmount);
			}
		}
	}

	public int IncomingStock(string goodId)
	{
		return Inventory.ReservedCapacity(goodId) - _linked.Inventory.AmountInStock(goodId);
	}

	public void OnEnterFinishedState()
	{
		Inventory.Enable();
		Inventory.InventoryStockChanged += OnInventoryStockChanged;
	}

	public void OnExitFinishedState()
	{
		Inventory.Disable();
		Inventory.InventoryStockChanged -= OnInventoryStockChanged;
	}

	private void OnInventoryStockChanged(object sender, InventoryStockChangedEventArgs e)
	{
		if (e.GoodAmount.Amount > 0)
		{
			_linked.Reserve(e.GoodAmount);
			TransferStock(e.GoodAmount.GoodId, e.GoodAmount.Amount);
		}
		else if (e.GoodAmount.Amount < 0)
		{
			_linked.Unreserve(new GoodAmount(e.GoodAmount.GoodId, -e.GoodAmount.Amount));
		}
	}

	private void OnBuildingLinked(object sender, LinkedBuilding e)
	{
		_linked = e.GetComponent<DistrictCrossingInventory>();
	}

	private void ReserveStock()
	{
		foreach (GoodAmount item in Inventory.Stock)
		{
			_linked.Reserve(item);
		}
	}

	private void GiveStock(GoodAmount goodAmount)
	{
		using (_mirrorOperationLock.Lock())
		{
			Inventory.GiveImported(goodAmount);
		}
	}

	private void Reserve(GoodAmount goodAmount)
	{
		_linkedInventories.Reserve(Inventory, goodAmount);
	}

	private void Unreserve(GoodAmount goodAmount)
	{
		_linkedInventories.Unreserve(Inventory, goodAmount);
	}
}
