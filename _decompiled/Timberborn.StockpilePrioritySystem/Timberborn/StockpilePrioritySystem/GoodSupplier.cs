using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.StockpilePrioritySystem;

public class GoodSupplier : BaseComponent, IPersistentEntity
{
	private static readonly ComponentKey GoodSupplierKey = new ComponentKey("GoodSupplier");

	private static readonly PropertyKey<bool> IsSupplyingKey = new PropertyKey<bool>("IsSupplying");

	public bool IsSupplying { get; private set; }

	public event EventHandler GoodSupplyingChanged;

	public void Save(IEntitySaver entitySaver)
	{
		if (IsSupplying)
		{
			entitySaver.GetComponent(GoodSupplierKey).Set(IsSupplyingKey, IsSupplying);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(GoodSupplierKey, out var objectLoader))
		{
			IsSupplying = objectLoader.Get(IsSupplyingKey);
		}
	}

	public void EnableSupplying()
	{
		IsSupplying = true;
		GoodSupplyingChanged?.Invoke(this, EventArgs.Empty);
	}

	public void DisableSupplying()
	{
		IsSupplying = false;
		GoodSupplyingChanged?.Invoke(this, EventArgs.Empty);
	}
}
