using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntityNaming;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.RelationSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Automation;

public class Automator : BaseComponent, IAwakableComponent, IInitializableEntity, IPostLoadableEntity, IDeletableEntity, IFinishedStateListener, IPersistentEntity, IRelationOwner
{
	private static readonly ComponentKey AutomatorKey = new ComponentKey("Automator");

	private static readonly PropertyKey<AutomatorState> StateKey = new PropertyKey<AutomatorState>("State");

	internal int Indegree;

	internal long PlanVersion;

	internal bool PostponedNotifyListeners;

	private readonly AutomationRunner _automationRunner;

	private BlockObject _blockObject;

	private NamedEntity _namedEntity;

	private EntityComponent _entityComponent;

	private ITransmitter _transmitter;

	private ISamplingTransmitter _samplingTransmitter;

	private ICombinationalTransmitter _combinationalTransmitter;

	private ISequentialTransmitter _sequentialTransmitter;

	private List<ITerminal> _terminals;

	private List<IAutomatorListener> _listeners;

	private readonly List<AutomatorConnection> _inputConnections = new List<AutomatorConnection>();

	private readonly List<AutomatorConnection> _outputConnections = new List<AutomatorConnection>();

	private bool _awoken;

	private AutomatorState _state;

	public int Evaluations { get; private set; }

	public ReadOnlyList<AutomatorConnection> InputConnections { get; }

	public ReadOnlyList<AutomatorConnection> OutputConnections { get; }

	public bool IsCyclicOrBlocked { get; private set; }

	public AutomatorPartition Partition { get; internal set; }

	internal bool RegisteredForRunning { get; private set; }

	public string AutomatorName => _namedEntity.EntityName;

	public string AutomatorId => _entityComponent.EntityId.ToString();

	public NamedEntitySortingKey SortingKey => _namedEntity.SortingKey;

	public bool IsTransmitter => _transmitter != null;

	public AutomatorState State => _state switch
	{
		AutomatorState.Off => AutomatorState.Off, 
		AutomatorState.On => _blockObject.IsFinished ? AutomatorState.On : AutomatorState.Off, 
		AutomatorState.Error => AutomatorState.Error, 
		_ => throw new Exception($"Unexpected state {_state}"), 
	};

	public AutomatorState UnfinishedState => _state;

	public int Usages => _outputConnections.Count;

	public bool IsProcessingNewInput => _sequentialTransmitter?.IsProcessingNewInput ?? false;

	internal bool IsSamplingTransmitter => _samplingTransmitter != null;

	internal bool IsCombinationalTransmitter => _combinationalTransmitter != null;

	internal bool IsSequentialTransmitter => _sequentialTransmitter != null;

	internal bool IsTerminal
	{
		get
		{
			if (_terminals != null)
			{
				return !_terminals.IsEmpty();
			}
			return false;
		}
	}

	private bool CanHaveInput
	{
		get
		{
			if (!IsCombinationalTransmitter && !IsSequentialTransmitter)
			{
				return IsTerminal;
			}
			return true;
		}
	}

	public event EventHandler IsCyclicOrBlockedChanged;

	public event EventHandler RelationsChanged;

	internal Automator(AutomationRunner automationRunner)
	{
		_automationRunner = automationRunner;
		InputConnections = _inputConnections.AsReadOnlyList();
		OutputConnections = _outputConnections.AsReadOnlyList();
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_namedEntity = GetComponent<NamedEntity>();
		_entityComponent = GetComponent<EntityComponent>();
		_transmitter = GetComponent<ITransmitter>();
		_samplingTransmitter = _transmitter as ISamplingTransmitter;
		_combinationalTransmitter = _transmitter as ICombinationalTransmitter;
		_sequentialTransmitter = _transmitter as ISequentialTransmitter;
		_terminals = GetComponentsAllocating<ITerminal>();
		_listeners = GetComponentsAllocating<IAutomatorListener>();
		ValidateAwake();
		DisableComponent();
		_awoken = true;
	}

	public void InitializeEntity()
	{
		_automationRunner.Register(this);
		RegisteredForRunning = true;
		SchedulePartition();
	}

	public void PostLoadEntity()
	{
		if (IsSamplingTransmitter)
		{
			Sample();
		}
		if (_state != AutomatorState.Off)
		{
			NotifyOrPostponeListeners();
		}
	}

	public void DeleteEntity()
	{
		while (!_inputConnections.IsEmpty())
		{
			List<AutomatorConnection> inputConnections = _inputConnections;
			inputConnections[inputConnections.Count - 1].Remove();
		}
		while (!_outputConnections.IsEmpty())
		{
			List<AutomatorConnection> outputConnections = _outputConnections;
			outputConnections[outputConnections.Count - 1].Disconnect();
		}
		RegisteredForRunning = false;
		_automationRunner.Unregister(this);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		SchedulePartition();
		if (_state != AutomatorState.Off)
		{
			NotifyListenersNow();
		}
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public AutomatorConnection AddInput()
	{
		ValidateAddInput();
		AutomatorConnection automatorConnection = new AutomatorConnection(this, _automationRunner);
		_inputConnections.Add(automatorConnection);
		return automatorConnection;
	}

	public void SetState(bool state)
	{
		SetStateInternal(state ? AutomatorState.On : AutomatorState.Off);
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (IsTransmitter)
		{
			entitySaver.GetComponent(AutomatorKey).Set(StateKey, _state);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(AutomatorKey, out var objectLoader) && IsTransmitter && objectLoader.Has(StateKey))
		{
			_state = objectLoader.Get(StateKey);
		}
	}

	public IEnumerable<BaseComponent> GetRelations()
	{
		for (int i = 0; i < _inputConnections.Count; i++)
		{
			Automator transmitter = _inputConnections[i].Transmitter;
			if (transmitter != this)
			{
				yield return transmitter;
			}
		}
		for (int i = 0; i < _outputConnections.Count; i++)
		{
			Automator receiver = _outputConnections[i].Receiver;
			if (receiver != this)
			{
				yield return receiver;
			}
		}
	}

	internal void ConnectToOutput(AutomatorConnection automatorConnection)
	{
		_outputConnections.Add(automatorConnection);
	}

	internal void DisconnectFromOutput(AutomatorConnection automatorConnection)
	{
		_outputConnections.Remove(automatorConnection);
	}

	internal void RemoveInput(AutomatorConnection automatorConnection)
	{
		_inputConnections.Remove(automatorConnection);
	}

	internal void SetCyclicOrBlocked(bool value)
	{
		if (IsCyclicOrBlocked != value)
		{
			IsCyclicOrBlocked = value;
			IsCyclicOrBlockedChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	internal void Sample()
	{
		_samplingTransmitter.Sample();
	}

	internal void EvaluateCombinational()
	{
		if (IsCyclicOrBlocked)
		{
			SetStateInternal(AutomatorState.Error);
		}
		else
		{
			_combinationalTransmitter.Evaluate();
		}
		Evaluations++;
	}

	internal void EvaluateNext()
	{
		if (base.Enabled)
		{
			_sequentialTransmitter.EvaluateNext();
			Evaluations++;
		}
	}

	internal void CommitTick()
	{
		if (base.Enabled)
		{
			_sequentialTransmitter.CommitTick();
		}
	}

	internal void EvaluateTerminal()
	{
		if (base.Enabled)
		{
			for (int i = 0; i < _terminals.Count; i++)
			{
				_terminals[i].Evaluate();
			}
			Evaluations++;
		}
	}

	internal void NotifyListenersNow()
	{
		for (int i = 0; i < _listeners.Count; i++)
		{
			_listeners[i].OnAutomatorStateChanged();
		}
	}

	internal void OnInputReconnected()
	{
		SchedulePartition();
		RelationsChanged?.Invoke(this, EventArgs.Empty);
	}

	private void SetStateInternal(AutomatorState newState)
	{
		ValidateSetState();
		if (_state != newState)
		{
			_state = newState;
			if (_blockObject.IsFinished)
			{
				SchedulePartition();
			}
			NotifyOrPostponeListeners();
		}
	}

	private void SchedulePartition()
	{
		if (Partition != null)
		{
			_automationRunner.Schedule(Partition);
		}
	}

	private void NotifyOrPostponeListeners()
	{
		Partition?.NotifyOrPostponeAutomatorListeners(this);
	}

	private void ValidateAwake()
	{
		if (_transmitter != null && !_terminals.IsEmpty())
		{
			throw new Exception("Automator (" + base.Name + ") cannot be both a transmitter and a terminal.");
		}
		if (_transmitter == null && _terminals.IsEmpty())
		{
			throw new Exception("Automator (" + base.Name + ") must be either a transmitter or a terminal by supplying a component which implements one of: ITransmitter, ISamplingTransmitter, ICombinationalTransmitter, ISequentialTransmitter, or one or more ITerminal components.");
		}
		if (!CanHaveInput && !_inputConnections.IsEmpty())
		{
			throw new Exception("Automator (" + base.Name + ") has inputs but is not combinational, sequential or terminal.");
		}
	}

	private void ValidateAddInput()
	{
		if (_awoken && !CanHaveInput)
		{
			throw new InvalidOperationException("Trying to add input to Automator (" + base.Name + ") which is not combinational, sequential or terminal.");
		}
	}

	private void ValidateSetState()
	{
		if (_awoken && !IsTransmitter)
		{
			throw new InvalidOperationException("Trying to call SetState on a non-transmitter Automator (" + base.Name + ").");
		}
	}
}
