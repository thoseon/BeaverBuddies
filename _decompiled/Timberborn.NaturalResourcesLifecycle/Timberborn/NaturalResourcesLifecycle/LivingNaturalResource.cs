using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.NaturalResourcesLifecycle;

public class LivingNaturalResource : BaseComponent, IPersistentEntity, IInitializableEntity
{
	private static readonly ComponentKey LivingNaturalResourceKey = new ComponentKey("LivingNaturalResource");

	private static readonly PropertyKey<bool> IsDeadKey = new PropertyKey<bool>("IsDead");

	public bool IsDead { get; private set; }

	public event EventHandler Died;

	public event EventHandler ReversedDeath;

	public void InitializeEntity()
	{
		if (IsDead)
		{
			InternalDie();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (IsDead)
		{
			entitySaver.GetComponent(LivingNaturalResourceKey).Set(IsDeadKey, IsDead);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(LivingNaturalResourceKey, out var objectLoader))
		{
			IsDead = objectLoader.Get(IsDeadKey);
		}
	}

	public void Die()
	{
		if (!IsDead)
		{
			InternalDie();
		}
	}

	public void ReverseDeath()
	{
		if (IsDead)
		{
			IsDead = false;
			ReversedDeath?.Invoke(this, EventArgs.Empty);
		}
	}

	private void InternalDie()
	{
		IsDead = true;
		Died?.Invoke(this, EventArgs.Empty);
	}
}
