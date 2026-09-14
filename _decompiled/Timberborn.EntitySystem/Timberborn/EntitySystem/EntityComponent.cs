using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.EntitySystem;

public class EntityComponent : BaseComponent, IAwakableComponent
{
	private enum EntityState
	{
		Uninitialized,
		Initializing,
		Initialized,
		Deleted
	}

	private readonly EventBus _eventBus;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private EntityState _entityState;

	private bool _deleteAfterInitialization;

	public Guid EntityId { get; private set; }

	public List<IRegisteredComponent> RegisteredComponents { get; } = new List<IRegisteredComponent>();

	public bool Initialized => _entityState == EntityState.Initialized;

	public bool Deleted => _entityState == EntityState.Deleted;

	public EntityComponent(EventBus eventBus, EntityComponentRegistry entityComponentRegistry)
	{
		_eventBus = eventBus;
		_entityComponentRegistry = entityComponentRegistry;
	}

	public void Awake()
	{
		GetComponents(RegisteredComponents);
	}

	public void PreInitialize()
	{
		if (Deleted)
		{
			return;
		}
		foreach (IPreInitializableEntity item in GetComponentsAllocating<IPreInitializableEntity>())
		{
			item.PreInitializeEntity();
		}
	}

	public void Initialize()
	{
		if (Deleted)
		{
			return;
		}
		_entityState = EntityState.Initializing;
		foreach (IInitializableEntity item in GetComponentsAllocating<IInitializableEntity>())
		{
			item.InitializeEntity();
		}
		_entityComponentRegistry.Register(this);
		_eventBus.Post(new EntityInitializedEvent(this));
		_entityState = EntityState.Initialized;
		if (_deleteAfterInitialization)
		{
			InternalDelete();
		}
	}

	public void PostInitialize()
	{
		if (Deleted)
		{
			return;
		}
		foreach (IPostInitializableEntity item in GetComponentsAllocating<IPostInitializableEntity>())
		{
			item.PostInitializeEntity();
		}
	}

	public void PostLoad()
	{
		if (Deleted)
		{
			return;
		}
		foreach (IPostLoadableEntity item in GetComponentsAllocating<IPostLoadableEntity>())
		{
			item.PostLoadEntity();
		}
	}

	internal void InitializeIfUninitialized()
	{
		if (_entityState == EntityState.Uninitialized)
		{
			PreInitialize();
			Initialize();
			PostInitialize();
			PostLoad();
		}
	}

	internal void Delete()
	{
		if (_entityState == EntityState.Initialized)
		{
			InternalDelete();
			return;
		}
		_deleteAfterInitialization = true;
		InitializeIfUninitialized();
	}

	internal void SetEntityId(Guid id)
	{
		if (EntityId != Guid.Empty)
		{
			throw new InvalidOperationException(base.Name + " has already initialized EntityId!");
		}
		EntityId = id;
	}

	private void InternalDelete()
	{
		if (Deleted)
		{
			return;
		}
		_entityState = EntityState.Deleted;
		foreach (IDeletableEntity item in GetComponentsAllocating<IDeletableEntity>())
		{
			item.DeleteEntity();
		}
		_entityComponentRegistry.Unregister(this);
		_eventBus.Post(new EntityDeletedEvent(this));
		_eventBus.Unregister(base.AllComponents);
	}
}
