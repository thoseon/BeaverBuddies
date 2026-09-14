using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class Timer : BaseComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity, IDuplicable<Timer>, IDuplicable, ISequentialTransmitter, ITransmitter
{
	private static readonly ComponentKey TimerKey = new ComponentKey("Timer");

	private static readonly PropertyKey<TimerMode> ModeKey = new PropertyKey<TimerMode>("Mode");

	private static readonly PropertyKey<Automator> InputKey = new PropertyKey<Automator>("Input");

	private static readonly PropertyKey<Automator> ResetInputKey = new PropertyKey<Automator>("ResetInput");

	private static readonly PropertyKey<TimerInterval> TimerIntervalAKey = new PropertyKey<TimerInterval>("TimerIntervalA");

	private static readonly PropertyKey<TimerInterval> TimerIntervalBKey = new PropertyKey<TimerInterval>("TimerIntervalB");

	private static readonly PropertyKey<bool> StateKey = new PropertyKey<bool>("State");

	private static readonly PropertyKey<bool> PreviousInputStateKey = new PropertyKey<bool>("PreviousInputState");

	private static readonly PropertyKey<int> CounterKey = new PropertyKey<int>("Counter");

	private readonly ReferenceSerializer _referenceSerializer;

	private readonly TimerIntervalSerializer _timerIntervalSerializer;

	private readonly TimerIntervalFactory _timerIntervalFactory;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private AutomatorConnection _input;

	private AutomatorConnection _resetInput;

	private Automator _automator;

	private bool _state;

	private bool _nextState;

	private bool _previousInputState;

	private bool _nextPreviousInputState;

	private int _counter;

	private int _nextCounter;

	private bool? _nextRandomState;

	public TimerMode Mode { get; private set; }

	public TimerInterval TimerIntervalA { get; private set; }

	public TimerInterval TimerIntervalB { get; private set; }

	public Automator Input => _input.Transmitter;

	public Automator ResetInput => _resetInput.Transmitter;

	public bool UsesIntervalB
	{
		get
		{
			TimerMode mode = Mode;
			if (mode == TimerMode.Delay || mode == TimerMode.Oscillator)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsProcessingNewInput
	{
		get
		{
			if (_state != _nextState)
			{
				return Mode != TimerMode.Random;
			}
			return false;
		}
	}

	private int TimeA => TimerIntervalA.Ticks;

	private int TimeB => TimerIntervalB.Ticks;

	private bool InputState => _input.BooleanState;

	private bool ResetInputState => _resetInput.BooleanState;

	private bool NextRandomStateCached
	{
		get
		{
			bool valueOrDefault = _nextRandomState == true;
			if (!_nextRandomState.HasValue)
			{
				valueOrDefault = _randomNumberGenerator.CheckProbability(0.5f);
				_nextRandomState = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public event EventHandler TimerTicked;

	public Timer(ReferenceSerializer referenceSerializer, TimerIntervalSerializer timerIntervalSerializer, TimerIntervalFactory timerIntervalFactory, IRandomNumberGenerator randomNumberGenerator)
	{
		_referenceSerializer = referenceSerializer;
		_timerIntervalSerializer = timerIntervalSerializer;
		_timerIntervalFactory = timerIntervalFactory;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		_automator = GetComponent<Automator>();
		_input = _automator.AddInput();
		_resetInput = _automator.AddInput();
		TimerIntervalA = _timerIntervalFactory.CreateFromHours(1f, IntervalType.Hours);
		TimerIntervalB = _timerIntervalFactory.CreateFromHours(1f, IntervalType.Hours);
	}

	public void InitializeEntity()
	{
		UpdateOutputState();
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(TimerKey);
		component.Set(ModeKey, Mode);
		component.Set(TimerIntervalAKey, TimerIntervalA, _timerIntervalSerializer);
		if (UsesIntervalB)
		{
			component.Set(TimerIntervalBKey, TimerIntervalB, _timerIntervalSerializer);
		}
		if ((bool)Input)
		{
			component.Set(InputKey, Input, _referenceSerializer.Of<Automator>());
		}
		if ((bool)ResetInput)
		{
			component.Set(ResetInputKey, ResetInput, _referenceSerializer.Of<Automator>());
		}
		if (_state)
		{
			component.Set(StateKey, _state);
		}
		if (_previousInputState)
		{
			component.Set(PreviousInputStateKey, _previousInputState);
		}
		component.Set(CounterKey, _counter);
	}

	[BackwardCompatible(2026, 2, 5, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(TimerKey);
		Mode = component.Get(ModeKey);
		if (component.Has(TimerIntervalAKey))
		{
			TimerIntervalA = component.Get(TimerIntervalAKey, _timerIntervalSerializer);
		}
		if (component.Has(TimerIntervalBKey))
		{
			TimerIntervalB = component.Get(TimerIntervalBKey, _timerIntervalSerializer);
		}
		if (component.Has(InputKey) && component.GetObsoletable(InputKey, _referenceSerializer.Of<Automator>(), out var value))
		{
			SetInput(value);
		}
		if (component.Has(ResetInputKey) && component.GetObsoletable(ResetInputKey, _referenceSerializer.Of<Automator>(), out var value2))
		{
			SetResetInput(value2);
		}
		_state = component.Has(StateKey) && component.Get(StateKey);
		_previousInputState = component.Has(PreviousInputStateKey) && component.Get(PreviousInputStateKey);
		if (component.Has(CounterKey))
		{
			_counter = component.Get(CounterKey);
		}
	}

	public void DuplicateFrom(Timer source)
	{
		TimerIntervalA.DuplicateFrom(source.TimerIntervalA);
		TimerIntervalB.DuplicateFrom(source.TimerIntervalB);
		SetInput(source.Input);
		SetResetInput(source.ResetInput);
		SetMode(source.Mode);
	}

	public float GetProgress(out bool isCountingTimeB)
	{
		isCountingTimeB = IsCountingTimeB();
		return (float)_counter / (float)(isCountingTimeB ? TimeB : TimeA);
	}

	public int GetTicksLeft()
	{
		int num = (IsCountingTimeB() ? TimeB : TimeA);
		return Math.Max(0, num - _counter);
	}

	public bool IsUsingTicks()
	{
		if (!IsCountingTimeB())
		{
			return TimerIntervalA.Type == IntervalType.Ticks;
		}
		return TimerIntervalB.Type == IntervalType.Ticks;
	}

	public void SetMode(TimerMode timerMode)
	{
		if (timerMode != Mode)
		{
			Mode = timerMode;
			Reset();
		}
	}

	public void SetInput(Automator automator)
	{
		_input.Connect(automator);
	}

	public void SetResetInput(Automator automator)
	{
		_resetInput.Connect(automator);
	}

	public void EvaluateNext()
	{
		EvaluateTimer();
		_nextPreviousInputState = InputState;
	}

	public void CommitTick()
	{
		_state = _nextState;
		_counter = _nextCounter;
		_previousInputState = _nextPreviousInputState;
		_nextRandomState = null;
		UpdateOutputState();
		TimerTicked?.Invoke(this, EventArgs.Empty);
	}

	public void Reset()
	{
		_state = false;
		_counter = 0;
		UpdateOutputState();
	}

	private void UpdateOutputState()
	{
		_automator.SetState(_state);
	}

	private void EvaluateTimer()
	{
		if (ResetInputState)
		{
			_nextState = false;
			_nextCounter = 0;
			return;
		}
		switch (Mode)
		{
		case TimerMode.Delay:
			EvaluateDelay();
			break;
		case TimerMode.Pulse:
			EvaluatePulse();
			break;
		case TimerMode.Oscillator:
			EvaluateOscillator();
			break;
		case TimerMode.Accumulator:
			EvaluateAccumulator();
			break;
		case TimerMode.Random:
			EvaluateRandom();
			break;
		default:
			throw new ArgumentOutOfRangeException("Mode", Mode, null);
		}
	}

	private void EvaluateDelay()
	{
		if (InputState)
		{
			_nextCounter = (_state ? TimeA : (_counter + 1));
			_nextState = _state || _nextCounter >= TimeA;
		}
		else if (_state)
		{
			_nextState = (_previousInputState ? (TimeB > 1) : (_counter + 1 < TimeB));
			_nextCounter = (_nextState ? (_previousInputState ? 1 : (_counter + 1)) : 0);
		}
		else
		{
			_nextState = false;
			_nextCounter = 0;
		}
	}

	private void EvaluatePulse()
	{
		if (InputState && !_previousInputState)
		{
			_nextCounter = 1;
			_nextState = true;
		}
		else if (_state)
		{
			_nextState = _counter < TimeA;
			_nextCounter = (_nextState ? (_counter + 1) : 0);
		}
		else
		{
			_nextCounter = 0;
			_nextState = false;
		}
	}

	private void EvaluateOscillator()
	{
		if (InputState)
		{
			_nextState = ((_state || !_previousInputState) ? (_counter < TimeA) : (_counter >= TimeB));
			_nextCounter = ((_nextState != _state) ? 1 : (_counter + 1));
		}
		else
		{
			_nextCounter = 0;
			_nextState = false;
		}
	}

	private void EvaluateAccumulator()
	{
		if (_state)
		{
			_nextCounter = TimeA;
			_nextState = true;
		}
		else
		{
			_nextCounter = (InputState ? (_counter + 1) : _counter);
			_nextState = _nextCounter >= TimeA;
		}
	}

	private void EvaluateRandom()
	{
		if (InputState)
		{
			_nextCounter = ((_counter >= TimeA) ? 1 : (_counter + 1));
			_nextState = ((_nextCounter == 1) ? NextRandomStateCached : _state);
		}
		else
		{
			_nextCounter = 0;
			_nextState = false;
		}
	}

	private bool IsCountingTimeB()
	{
		if (UsesIntervalB)
		{
			return Mode switch
			{
				TimerMode.Delay => _state && !InputState && !_previousInputState, 
				TimerMode.Oscillator => !_state && InputState && _previousInputState, 
				_ => throw new ArgumentOutOfRangeException("Mode", Mode, null), 
			};
		}
		return false;
	}
}
