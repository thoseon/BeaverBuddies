using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.InventorySystem;

public class GoodReserver : BaseComponent, IPostLoadableEntity, IPersistentEntity
{
	private static readonly ComponentKey GoodReserverKey = new ComponentKey("GoodReserver");

	private static readonly PropertyKey<GoodReservation> StockReservationKey = new PropertyKey<GoodReservation>("StockReservation");

	private static readonly PropertyKey<GoodReservation> CapacityReservationKey = new PropertyKey<GoodReservation>("CapacityReservation");

	private readonly GoodReservationValueSerializer _goodReservationValueSerializer;

	public GoodReservation StockReservation { get; private set; }

	public GoodReservation CapacityReservation { get; private set; }

	public bool HasReservedCapacity
	{
		get
		{
			if ((bool)CapacityReservation.Inventory)
			{
				return CapacityReservation.Inventory?.Enabled ?? false;
			}
			return false;
		}
	}

	public bool HasReservedStock
	{
		get
		{
			if ((bool)StockReservation.Inventory)
			{
				return StockReservation.Inventory?.Enabled ?? false;
			}
			return false;
		}
	}

	public GoodReserver(GoodReservationValueSerializer goodReservationValueSerializer)
	{
		_goodReservationValueSerializer = goodReservationValueSerializer;
	}

	public void PostLoadEntity()
	{
		ResolveLoadedReservations();
		GetComponent<Character>().Died += OnDied;
	}

	public void ReserveCapacity(Inventory inventory, GoodAmount capacity)
	{
		ThrowIfInventoryNotEnabled(inventory);
		UnreserveCapacity();
		inventory.ReserveCapacity(capacity);
		CapacityReservation = new GoodReservation(inventory, capacity, fixedAmount: true, consumeGood: false);
	}

	public void UnreserveCapacity()
	{
		if ((bool)CapacityReservation.Inventory && !CapacityReservation.Inventory.GetComponent<EntityComponent>().Deleted)
		{
			CapacityReservation.Inventory.UnreserveCapacity(CapacityReservation.GoodAmount);
		}
		CapacityReservation = default(GoodReservation);
	}

	public void ReserveExactStockAmount(Inventory inventory, GoodAmount good)
	{
		ReserveStock(inventory, good, fixedAmount: true, consumeStock: false);
	}

	public void ReserveNotLessThanStockAmount(Inventory inventory, GoodAmount good)
	{
		ReserveStock(inventory, good, fixedAmount: false, consumeStock: false);
	}

	public void ReserveNotLessThanStockAmountConsuming(Inventory inventory, GoodAmount good)
	{
		ReserveStock(inventory, good, fixedAmount: false, consumeStock: true);
	}

	public void UnreserveStock()
	{
		if ((bool)StockReservation.Inventory && !StockReservation.Inventory.GetComponent<EntityComponent>().Deleted)
		{
			StockReservation.Inventory.UnreserveStock(StockReservation.GoodAmount);
		}
		StockReservation = default(GoodReservation);
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (HasReservedStock || HasReservedCapacity)
		{
			IObjectSaver component = entitySaver.GetComponent(GoodReserverKey);
			if (HasReservedStock)
			{
				component.Set(StockReservationKey, StockReservation, _goodReservationValueSerializer);
			}
			if (HasReservedCapacity)
			{
				component.Set(CapacityReservationKey, CapacityReservation, _goodReservationValueSerializer);
			}
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(GoodReserverKey, out var objectLoader))
		{
			if (objectLoader.Has(StockReservationKey) && objectLoader.GetObsoletable(StockReservationKey, _goodReservationValueSerializer, out var value))
			{
				StockReservation = value;
			}
			if (objectLoader.Has(CapacityReservationKey) && objectLoader.GetObsoletable(CapacityReservationKey, _goodReservationValueSerializer, out var value2))
			{
				CapacityReservation = value2;
			}
		}
	}

	private void ResolveLoadedReservations()
	{
		if (CapacityReservation.Inventory != null)
		{
			if (CapacityReservation.Inventory.HasUnreservedCapacity(CapacityReservation.GoodAmount))
			{
				CapacityReservation.Inventory.ReserveCapacity(CapacityReservation.GoodAmount);
			}
			else
			{
				LogReservationResolvingWarning(CapacityReservation, "capacity");
				CapacityReservation = default(GoodReservation);
			}
		}
		if (StockReservation.Inventory != null)
		{
			if (StockReservation.Inventory.HasUnreservedStock(StockReservation.GoodAmount))
			{
				StockReservation.Inventory.ReserveStock(StockReservation.GoodAmount);
				return;
			}
			LogReservationResolvingWarning(StockReservation, "good");
			StockReservation = default(GoodReservation);
		}
	}

	private void OnDied(object sender, EventArgs e)
	{
		UnreserveCapacity();
		UnreserveStock();
	}

	private void ReserveStock(Inventory inventory, GoodAmount good, bool fixedAmount, bool consumeStock)
	{
		ThrowIfInventoryNotEnabled(inventory);
		UnreserveStock();
		inventory.ReserveStock(good);
		StockReservation = new GoodReservation(inventory, good, fixedAmount, consumeStock);
	}

	private static void ThrowIfInventoryNotEnabled(Inventory inventory)
	{
		if (!inventory.Enabled)
		{
			throw new ArgumentException(string.Format("Provided {0} is not enabled! {1}", "Inventory", inventory));
		}
	}

	private void LogReservationResolvingWarning(GoodReservation goodReservation, string reservationType)
	{
		Debug.LogWarning("While loading " + base.Name + " couldn't reserve " + reservationType + " " + $"for {goodReservation.GoodAmount} at {goodReservation.Inventory.Name} " + "and ignored this reservation of " + reservationType + ".");
	}
}
