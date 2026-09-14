using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.GameCycleSystem;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.ActivatorSystem;

public class TimedComponentActivator : TickableComponent, IPersistentEntity, IAwakableComponent, IInitializableEntity, IDuplicable<TimedComponentActivator>, IDuplicable, IDeletableEntity
{
	private static readonly ComponentKey TimeActivatedComponentKey = new ComponentKey("TimeActivatedComponent");

	private static readonly PropertyKey<bool> IsEnabledKey = new PropertyKey<bool>("IsEnabled");

	private static readonly PropertyKey<int> CyclesUntilCountdownActivationKey = new PropertyKey<int>("CyclesUntilCountdownActivation");

	private static readonly PropertyKey<float> DaysUntilActivationKey = new PropertyKey<float>("DaysUntilActivation");

	private static readonly PropertyKey<float> DaysPassedKey = new PropertyKey<float>("DaysPassed");

	private readonly EventBus _eventBus;

	private readonly MapEditorMode _mapEditorMode;

	private readonly GameCycleService _gameCycleService;

	private readonly IDayNightCycle _dayNightCycle;

	private TimedComponentActivatorSpec _spec;

	private IActivableComponent _activableComponent;

	private float _daysPassed;

	private bool _wasActivated;

	public int CyclesUntilCountdownActivation { get; private set; }

	public float DaysUntilActivation { get; private set; }

	public bool IsEnabled { get; private set; }

	public bool CountdownIsActive
	{
		get
		{
			if (!_mapEditorMode.IsMapEditor && IsEnabled && _gameCycleService.Cycle >= CyclesUntilCountdownActivation)
			{
				return DaysPassedWithHours < DaysUntilActivation;
			}
			return false;
		}
	}

	public bool IsPastActivationTime
	{
		get
		{
			if (!_mapEditorMode.IsMapEditor && _gameCycleService.Cycle >= CyclesUntilCountdownActivation)
			{
				return DaysPassedWithHours >= DaysUntilActivation;
			}
			return false;
		}
	}

	public bool IsOptional => _spec.IsOptionallyActivable;

	public float ActivationProgress => DaysPassedWithHours / DaysUntilActivation;

	public float DaysLeftUntilActivation => DaysUntilActivation - DaysPassedWithHours;

	public bool IsDuplicable => _mapEditorMode.IsMapEditor;

	private float DaysPassedWithHours => _daysPassed + _dayNightCycle.DayProgress;

	public event EventHandler CountdownActivated;

	public event EventHandler Activated;

	public TimedComponentActivator(EventBus eventBus, MapEditorMode mapEditorMode, GameCycleService gameCycleService, IDayNightCycle dayNightCycle)
	{
		_eventBus = eventBus;
		_mapEditorMode = mapEditorMode;
		_gameCycleService = gameCycleService;
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_spec = GetComponent<TimedComponentActivatorSpec>();
		_activableComponent = GetComponent<IActivableComponent>();
		CyclesUntilCountdownActivation = _spec.CyclesUntilCountdownActivation;
		DaysUntilActivation = _spec.DaysUntilActivation;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(TimeActivatedComponentKey);
		component.Set(IsEnabledKey, IsEnabled);
		component.Set(CyclesUntilCountdownActivationKey, CyclesUntilCountdownActivation);
		component.Set(DaysUntilActivationKey, DaysUntilActivation);
		component.Set(DaysPassedKey, _daysPassed);
	}

	[BackwardCompatible(2025, 9, 16, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(TimeActivatedComponentKey, out var objectLoader))
		{
			IsEnabled = objectLoader.Get(IsEnabledKey);
			CyclesUntilCountdownActivation = objectLoader.Get(CyclesUntilCountdownActivationKey);
			DaysUntilActivation = objectLoader.Get(DaysUntilActivationKey);
			_daysPassed = objectLoader.Get(DaysPassedKey);
		}
	}

	public void InitializeEntity()
	{
		if (!IsOptional)
		{
			IsEnabled = true;
		}
		InitializeActivableComponent();
		_eventBus.Register(this);
	}

	public void DeleteEntity()
	{
		_eventBus.Unregister(this);
	}

	public void DuplicateFrom(TimedComponentActivator source)
	{
		if (IsOptional)
		{
			IsEnabled = source.IsEnabled;
		}
		CyclesUntilCountdownActivation = source.CyclesUntilCountdownActivation;
		DaysUntilActivation = source.DaysUntilActivation;
	}

	public override void Tick()
	{
		ActivateIfItsTime();
	}

	public void EnableActivator()
	{
		_activableComponent.Deactivate();
		IsEnabled = true;
	}

	public void DisableActivator()
	{
		_activableComponent.Activate();
		IsEnabled = false;
	}

	[OnEvent]
	public void OnCycleDayStarted(CycleDayStartedEvent cycleDayStartedEvent)
	{
		if (!_mapEditorMode.IsMapEditor && IsEnabled)
		{
			if (_gameCycleService.Cycle == CyclesUntilCountdownActivation && _gameCycleService.CycleDay == 1)
			{
				CountdownActivated?.Invoke(this, EventArgs.Empty);
			}
			else if (_gameCycleService.Cycle >= CyclesUntilCountdownActivation)
			{
				_daysPassed++;
			}
		}
	}

	public void SetCyclesUntilCountdownActivation(int cyclesUntilCountdownActivation)
	{
		CyclesUntilCountdownActivation = cyclesUntilCountdownActivation;
	}

	public void SetDaysUntilActivation(float daysUntilActivation)
	{
		DaysUntilActivation = daysUntilActivation;
	}

	private void InitializeActivableComponent()
	{
		if (IsEnabled)
		{
			if (IsPastActivationTime)
			{
				_activableComponent.Activate();
			}
			else
			{
				_activableComponent.Deactivate();
			}
		}
	}

	private void ActivateIfItsTime()
	{
		if (!_wasActivated && IsPastActivationTime)
		{
			_activableComponent.Activate();
			Activated?.Invoke(this, EventArgs.Empty);
			_wasActivated = true;
		}
	}
}
