using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.GameWonderCompletion;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Wonders;

public class Wonder : BaseComponent, IAwakableComponent, IPersistentEntity, IFinishedStateListener, IRegisteredComponent
{
	private static readonly ComponentKey WonderKey = new ComponentKey("Wonder");

	private static readonly PropertyKey<bool> IsActiveKey = new PropertyKey<bool>("IsActive");

	private readonly WonderCompletionCountdownStarter _wonderCompletionCountdownStarter;

	private readonly EventBus _eventBus;

	private readonly List<IWonderBlocker> _wonderBlockers = new List<IWonderBlocker>();

	public bool IsActive { get; private set; }

	public event EventHandler WonderActivated;

	public event EventHandler WonderDeactivated;

	public Wonder(WonderCompletionCountdownStarter wonderCompletionCountdownStarter, EventBus eventBus)
	{
		_wonderCompletionCountdownStarter = wonderCompletionCountdownStarter;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		GetComponents(_wonderBlockers);
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(WonderKey).Set(IsActiveKey, IsActive);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(WonderKey);
		IsActive = component.Get(IsActiveKey);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		_eventBus.Register(this);
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		_eventBus.Unregister(this);
	}

	public void Activate()
	{
		if (CanBeActivated())
		{
			IsActive = true;
			WonderActivated?.Invoke(this, EventArgs.Empty);
			_eventBus.Post(new WonderActivatedEvent());
		}
	}

	public void Deactivate()
	{
		IsActive = false;
		WonderDeactivated?.Invoke(this, EventArgs.Empty);
		_wonderCompletionCountdownStarter.BeginUnlockCountdown();
	}

	public bool CanBeActivated()
	{
		for (int i = 0; i < _wonderBlockers.Count; i++)
		{
			if (_wonderBlockers[i].IsWonderBlocked())
			{
				return false;
			}
		}
		return true;
	}
}
