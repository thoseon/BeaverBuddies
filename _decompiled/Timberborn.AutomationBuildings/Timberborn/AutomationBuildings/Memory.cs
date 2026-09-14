using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class Memory : BaseComponent, IAwakableComponent, IPersistentEntity, IInitializableEntity, IDuplicable<Memory>, IDuplicable, IFinishedStateListener, ISequentialTransmitter, ITransmitter
{
	private static readonly ComponentKey MemoryKey = new ComponentKey("Memory");

	private static readonly PropertyKey<MemoryMode> ModeKey = new PropertyKey<MemoryMode>("Mode");

	private static readonly PropertyKey<Automator> InputAKey = new PropertyKey<Automator>("InputA");

	private static readonly PropertyKey<Automator> InputBKey = new PropertyKey<Automator>("InputB");

	private static readonly PropertyKey<Automator> ResetInputKey = new PropertyKey<Automator>("ResetInput");

	private static readonly PropertyKey<bool> StateKey = new PropertyKey<bool>("State");

	private static readonly PropertyKey<bool> PreviousAStateKey = new PropertyKey<bool>("PreviousAState");

	private static readonly PropertyKey<bool> PreviousBStateKey = new PropertyKey<bool>("PreviousBState");

	private readonly ReferenceSerializer _referenceSerializer;

	private Automator _automator;

	private AutomatorConnection _inputA;

	private AutomatorConnection _inputB;

	private AutomatorConnection _resetInput;

	private bool _state;

	private bool _previousAState;

	private bool _previousBState;

	private bool _nextState;

	private bool _nextPreviousAState;

	private bool _nextPreviousBState;

	public MemoryMode Mode { get; private set; }

	public Automator InputA => _inputA.Transmitter;

	public Automator InputB => _inputB.Transmitter;

	public Automator ResetInput => _resetInput.Transmitter;

	public bool UsesInputB => Mode switch
	{
		MemoryMode.SetReset => false, 
		MemoryMode.Toggle => false, 
		MemoryMode.Latch => true, 
		MemoryMode.FlipFlop => true, 
		_ => throw new ArgumentOutOfRangeException($"Unexpected value: {Mode}"), 
	};

	public bool IsProcessingNewInput => _nextState != _state;

	private bool AState => _inputA.BooleanState;

	private bool BState => _inputB.BooleanState;

	private bool ResetState => _resetInput.BooleanState;

	private bool ARising
	{
		get
		{
			if (_inputA.BooleanState)
			{
				return !_previousAState;
			}
			return false;
		}
	}

	private bool BRising
	{
		get
		{
			if (_inputB.BooleanState)
			{
				return !_previousBState;
			}
			return false;
		}
	}

	public Memory(ReferenceSerializer referenceSerializer)
	{
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_automator = GetComponent<Automator>();
		_inputA = _automator.AddInput();
		_inputB = _automator.AddInput();
		_resetInput = _automator.AddInput();
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(MemoryKey);
		component.Set(ModeKey, Mode);
		if ((bool)InputA)
		{
			component.Set(InputAKey, InputA, _referenceSerializer.Of<Automator>());
		}
		if ((bool)InputB)
		{
			component.Set(InputBKey, InputB, _referenceSerializer.Of<Automator>());
		}
		if ((bool)ResetInput)
		{
			component.Set(ResetInputKey, ResetInput, _referenceSerializer.Of<Automator>());
		}
		if (_state)
		{
			component.Set(StateKey, _state);
		}
		if (_previousAState)
		{
			component.Set(PreviousAStateKey, _previousAState);
		}
		if (_previousBState)
		{
			component.Set(PreviousBStateKey, _previousBState);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader objectLoader = (entityLoader.TryGetComponent(MemoryKey, out var objectLoader2) ? objectLoader2 : entityLoader.GetComponent(new ComponentKey("Latch")));
		Mode = objectLoader.Get(ModeKey);
		if (objectLoader.Has(InputAKey) && objectLoader.GetObsoletable(InputAKey, _referenceSerializer.Of<Automator>(), out var value))
		{
			_inputA.Connect(value);
		}
		if (UsesInputB && objectLoader.Has(InputBKey) && objectLoader.GetObsoletable(InputBKey, _referenceSerializer.Of<Automator>(), out var value2))
		{
			_inputB.Connect(value2);
		}
		if (objectLoader.Has(ResetInputKey) && objectLoader.GetObsoletable(ResetInputKey, _referenceSerializer.Of<Automator>(), out var value3))
		{
			_resetInput.Connect(value3);
		}
		_state = objectLoader.Has(StateKey) && objectLoader.Get(StateKey);
		_previousAState = objectLoader.Has(PreviousAStateKey) && objectLoader.Get(PreviousAStateKey);
		_previousBState = objectLoader.Has(PreviousBStateKey) && objectLoader.Get(PreviousBStateKey);
	}

	public void InitializeEntity()
	{
		UpdateOutputState();
	}

	public void DuplicateFrom(Memory source)
	{
		SetMode(source.Mode);
		_inputA.Connect(source.InputA);
		_inputB.Connect(source.InputB);
		_resetInput.Connect(source.ResetInput);
	}

	public void OnEnterFinishedState()
	{
		_previousAState = _inputA.BooleanState;
		_previousBState = _inputB.BooleanState;
	}

	public void OnExitFinishedState()
	{
	}

	public void SetMode(MemoryMode memoryMode)
	{
		Mode = memoryMode;
		if (!UsesInputB)
		{
			_inputB.Disconnect();
		}
		EvaluateNext();
	}

	public void SetInputA(Automator automator)
	{
		_inputA.Connect(automator);
	}

	public void SetInputB(Automator automator)
	{
		if (UsesInputB)
		{
			_inputB.Connect(automator);
		}
	}

	public void SetResetInput(Automator automator)
	{
		_resetInput.Connect(automator);
	}

	public void Reset()
	{
		_state = false;
		UpdateOutputState();
	}

	public void EvaluateNext()
	{
		bool flag = !ResetState;
		if (flag)
		{
			flag = Mode switch
			{
				MemoryMode.SetReset => (_state || AState) && !BState, 
				MemoryMode.Toggle => ARising ? (!_state) : _state, 
				MemoryMode.Latch => BState ? AState : _state, 
				MemoryMode.FlipFlop => BRising ? AState : _state, 
				_ => throw new ArgumentOutOfRangeException($"Unexpected value: {Mode}"), 
			};
		}
		_nextState = flag;
		_nextPreviousAState = _inputA.BooleanState;
		_nextPreviousBState = _inputB.BooleanState;
	}

	public void CommitTick()
	{
		_state = _nextState;
		_previousAState = _nextPreviousAState;
		_previousBState = _nextPreviousBState;
		UpdateOutputState();
	}

	private void UpdateOutputState()
	{
		_automator.SetState(_state);
	}
}
