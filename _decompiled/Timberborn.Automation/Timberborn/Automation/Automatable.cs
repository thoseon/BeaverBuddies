using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.Automation;

public class Automatable : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<Automatable>, IDuplicable, ITerminal
{
	private static readonly ComponentKey AutomatableKey = new ComponentKey("Automatable");

	private static readonly PropertyKey<Automator> InputKey = new PropertyKey<Automator>("Input");

	private readonly ReferenceSerializer _referenceSerializer;

	private BlockObject _blockObject;

	private AutomatorConnection _inputConnection;

	private readonly List<IAutomatableNeeder> _automatableNeeders = new List<IAutomatableNeeder>();

	private ConnectionState _lastNotifyState;

	public ConnectionState State => _inputConnection.State switch
	{
		ConnectionState.Disconnected => ConnectionState.Disconnected, 
		ConnectionState.Off => ConnectionState.Off, 
		ConnectionState.On => (!_blockObject.IsFinished) ? ConnectionState.Off : ConnectionState.On, 
		_ => throw new Exception($"Unexpected state {_inputConnection.State}"), 
	};

	public bool IsAutomated => _inputConnection.IsConnected;

	public Automator Input => _inputConnection.Transmitter;

	public event EventHandler InputStateChanged;

	public event EventHandler InputReconnected;

	public Automatable(ReferenceSerializer referenceSerializer)
	{
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_inputConnection = GetComponent<Automator>().AddInput();
		GetComponents(_automatableNeeders);
	}

	public void SetInput(Automator automator)
	{
		if (automator != _inputConnection.Transmitter)
		{
			_inputConnection.Connect(automator);
			NotifyInputReconnected();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_inputConnection.IsConnected)
		{
			entitySaver.GetComponent(AutomatableKey).Set(InputKey, Input, _referenceSerializer.Of<Automator>());
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(AutomatableKey, out var objectLoader) && objectLoader.GetObsoletable(InputKey, _referenceSerializer.Of<Automator>(), out var value))
		{
			_inputConnection.Connect(value);
		}
	}

	public void DuplicateFrom(Automatable source)
	{
		SetInput(source._inputConnection.Transmitter);
	}

	public void Evaluate()
	{
		if (_lastNotifyState != State)
		{
			_lastNotifyState = State;
			NotifyInputStateChanged();
		}
	}

	public bool IsNeeded()
	{
		foreach (IAutomatableNeeder automatableNeeder in _automatableNeeders)
		{
			if (automatableNeeder.NeedsAutomatable)
			{
				return true;
			}
		}
		return false;
	}

	private void NotifyInputStateChanged()
	{
		InputStateChanged?.Invoke(this, EventArgs.Empty);
	}

	private void NotifyInputReconnected()
	{
		InputReconnected?.Invoke(this, EventArgs.Empty);
	}
}
