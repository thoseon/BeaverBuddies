using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.StockpilePrioritySystem;

public class GoodObtainer : BaseComponent, IPersistentEntity
{
	private static readonly ComponentKey GoodObtainerKey = new ComponentKey("GoodObtainer");

	private static readonly PropertyKey<bool> IsObtainingKey = new PropertyKey<bool>("IsObtaining");

	public bool IsObtaining { get; private set; }

	public event EventHandler GoodObtainingChanged;

	public void Save(IEntitySaver entitySaver)
	{
		if (IsObtaining)
		{
			entitySaver.GetComponent(GoodObtainerKey).Set(IsObtainingKey, IsObtaining);
		}
	}

	[BackwardCompatible(2025, 7, 4, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(GoodObtainerKey, out var objectLoader))
		{
			IsObtaining = (objectLoader.Has(IsObtainingKey) ? objectLoader.Get(IsObtainingKey) : objectLoader.Get(new PropertyKey<bool>("GoodObtainingEnabled")));
		}
	}

	public void EnableObtaining()
	{
		IsObtaining = true;
		GoodObtainingChanged?.Invoke(this, EventArgs.Empty);
	}

	public void DisableObtaining()
	{
		IsObtaining = false;
		GoodObtainingChanged?.Invoke(this, EventArgs.Empty);
	}
}
