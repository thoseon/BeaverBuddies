using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.InventorySystem;

namespace Timberborn.LinkedBuildingSystem;

public class LinkedInventories : BaseComponent, IAwakableComponent
{
	private Inventories _inventories;

	private LinkedInventories _linked;

	private readonly MirrorOperationLock _mirrorOperationLock = new MirrorOperationLock();

	public void Awake()
	{
		_inventories = GetComponent<Inventories>();
		foreach (Inventory allInventory in _inventories.AllInventories)
		{
			allInventory.InventoryCapacityReservationChanged += OnInventoryCapacityReservationChanged;
		}
		GetComponent<LinkedBuilding>().BuildingLinked += OnBuildingLinked;
	}

	public void Reserve(Inventory inventory, GoodAmount goodAmount)
	{
		using (_mirrorOperationLock.Lock())
		{
			inventory.ReserveCapacity(goodAmount);
		}
	}

	public void Unreserve(Inventory inventory, GoodAmount goodAmount)
	{
		using (_mirrorOperationLock.Lock())
		{
			inventory.UnreserveCapacity(goodAmount);
		}
	}

	private void OnInventoryCapacityReservationChanged(object sender, InventoryReservationChangedEventArgs e)
	{
		if (_mirrorOperationLock.IsUnlocked)
		{
			MirrorCapacityReservation((Inventory)sender, e.GoodAmount);
		}
	}

	private void OnBuildingLinked(object sender, LinkedBuilding e)
	{
		_linked = e.GetComponent<LinkedInventories>();
		if (e.IsLinked)
		{
			MirrorReservations();
		}
	}

	private void MirrorCapacityReservation(Inventory inventory, GoodAmount goodAmount)
	{
		Inventory linkedInventory = GetLinkedInventory(inventory);
		if (goodAmount.Amount > 0)
		{
			_linked.Reserve(linkedInventory, goodAmount);
		}
		else
		{
			_linked.Unreserve(linkedInventory, new GoodAmount(goodAmount.GoodId, -goodAmount.Amount));
		}
	}

	private Inventory GetLinkedInventory(Inventory myInventory)
	{
		foreach (Inventory allInventory in _linked._inventories.AllInventories)
		{
			if (allInventory.ComponentName == myInventory.ComponentName)
			{
				return allInventory;
			}
		}
		throw new ArgumentOutOfRangeException($"Couldn't find linked inventory in {base.GameObject} for {myInventory.ComponentName}");
	}

	private void MirrorReservations()
	{
		foreach (Inventory allInventory in _inventories.AllInventories)
		{
			Inventory linkedInventory = GetLinkedInventory(allInventory);
			MirrorInventoryReservations(allInventory, linkedInventory);
		}
	}

	private void MirrorInventoryReservations(Inventory myInventory, Inventory linkedInventory)
	{
		ReadOnlyList<GoodAmount> readOnlyList = myInventory.ReservedCapacity();
		ReadOnlyList<GoodAmount> readOnlyList2 = linkedInventory.ReservedCapacity();
		foreach (GoodAmount item in readOnlyList)
		{
			_linked.Reserve(linkedInventory, item);
		}
		foreach (GoodAmount item2 in readOnlyList2)
		{
			Reserve(myInventory, item2);
		}
	}
}
