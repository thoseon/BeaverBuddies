using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.BlockSystem;

internal class BlockObjectState : BaseComponent, IAwakableComponent, IPersistentEntity, IPostInitializableEntity, IDeletableEntity
{
	private enum State
	{
		Unfinished,
		Finished,
		Preview
	}

	private static readonly ComponentKey BlockObjectStateKey = new ComponentKey("BlockObjectState");

	private static readonly PropertyKey<bool> FinishedKey = new PropertyKey<bool>("Finished");

	private readonly EventBus _eventBus;

	private BlockObject _blockObject;

	private State _state;

	private bool _initialized;

	public bool IsUnfinished => _state == State.Unfinished;

	public bool IsFinished => _state == State.Finished;

	public bool IsPreview => _state == State.Preview;

	public BlockObjectState(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void PostInitializeEntity()
	{
		if (!_initialized)
		{
			Initialize();
		}
	}

	public void Initialize()
	{
		try
		{
			Asserts.IsFalse(this, _initialized, "_initialized");
			NotifyOnStateEntered();
			_initialized = true;
		}
		catch (Exception innerException)
		{
			throw new Exception("Exception while initializing: " + base.Name, innerException);
		}
	}

	public void MarkAsFinished()
	{
		if (IsFinished)
		{
			ThrowCannotTransitionToSameState();
		}
		EnterState(State.Finished);
	}

	public void MarkAsPreview()
	{
		if (IsPreview)
		{
			ThrowCannotTransitionToSameState();
		}
		EnterState(State.Preview);
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (!IsFinished)
		{
			entitySaver.GetComponent(BlockObjectStateKey).Set(FinishedKey, IsFinished);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(BlockObjectStateKey, out var objectLoader))
		{
			bool flag = !objectLoader.Has(FinishedKey) || objectLoader.Get(FinishedKey);
			_state = (flag ? State.Finished : State.Unfinished);
		}
		else
		{
			_state = State.Finished;
		}
	}

	public void DeleteEntity()
	{
		if (!_initialized)
		{
			Initialize();
		}
		NotifyOnStateExited();
	}

	private void EnterState(State state)
	{
		NotifyOnStateExited();
		_state = state;
		if (_initialized)
		{
			NotifyOnStateEntered();
		}
	}

	private void NotifyOnStateEntered()
	{
		if (IsFinished)
		{
			foreach (IFinishedStateListener item in GetComponentsAllocating<IFinishedStateListener>())
			{
				item.OnEnterFinishedState();
			}
			_eventBus.Post(new EnteredFinishedStateEvent(_blockObject));
		}
		else if (IsUnfinished)
		{
			foreach (IUnfinishedStateListener item2 in GetComponentsAllocating<IUnfinishedStateListener>())
			{
				item2.OnEnterUnfinishedState();
			}
			_eventBus.Post(new EnteredUnfinishedStateEvent(_blockObject));
		}
		else if (!IsPreview)
		{
			throw new ArgumentOutOfRangeException("_state", _state, $"Unexpected {_state} value: {_state}");
		}
	}

	private void NotifyOnStateExited()
	{
		if (!_initialized)
		{
			return;
		}
		if (IsFinished)
		{
			foreach (IFinishedStateListener item in GetComponentsAllocating<IFinishedStateListener>())
			{
				item.OnExitFinishedState();
			}
			_eventBus.Post(new ExitedFinishedStateEvent(_blockObject));
		}
		else
		{
			if (!IsUnfinished)
			{
				return;
			}
			foreach (IUnfinishedStateListener item2 in GetComponentsAllocating<IUnfinishedStateListener>())
			{
				item2.OnExitUnfinishedState();
			}
			_eventBus.Post(new ExitedUnfinishedStateEvent(_blockObject));
		}
	}

	private void ThrowCannotTransitionToSameState()
	{
		throw new InvalidOperationException($"{base.Name} cannot transition to {_state} state. It is already in this state.");
	}
}
