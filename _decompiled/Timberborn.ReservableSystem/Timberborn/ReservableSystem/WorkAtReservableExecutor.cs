using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.ReservableSystem;

public abstract class WorkAtReservableExecutor : BaseComponent, IAwakableComponent, IPostInitializableEntity, IExecutor
{
	private static readonly ComponentKey WorkAtReservableExecutorKey = new ComponentKey("WorkAtReservableExecutor");

	private static readonly PropertyKey<float> FinishTimestampKey = new PropertyKey<float>("FinishTimestamp");

	private static readonly PropertyKey<bool> IsLegacyExecutorKey = new PropertyKey<bool>("IsLegacyExecutor");

	private readonly IDayNightCycle _dayNightCycle;

	private CharacterAnimator _characterAnimator;

	private Worker _worker;

	private float _finishTimestamp;

	private bool _startWorkingPostLoad;

	private bool _isLegacyExecutor;

	private string _animationTurnedOn;

	protected abstract string Animation { get; }

	protected abstract Reservable Reservable { get; }

	public event EventHandler WorkStarted;

	public event EventHandler<WorkFinishedEventArgs> WorkFinished;

	protected WorkAtReservableExecutor(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_characterAnimator = GetComponent<CharacterAnimator>();
		_worker = GetComponent<Worker>();
		Initialize();
	}

	public void PostInitializeEntity()
	{
		if (_startWorkingPostLoad)
		{
			if (Reservable != null)
			{
				StartWorking();
			}
			_startWorkingPostLoad = false;
		}
	}

	public bool IsLoadedLegacyExecutor()
	{
		if (_startWorkingPostLoad)
		{
			return _isLegacyExecutor;
		}
		return false;
	}

	public ExecutorStatus Tick(float deltaTimeInHours)
	{
		if (Reservable == null)
		{
			return Stop();
		}
		if (_dayNightCycle.PartialDayNumber >= _finishTimestamp)
		{
			if (!Reservable.IsDeleted && CanComplete())
			{
				return Complete();
			}
			return Stop();
		}
		PerformActionOnTick(_dayNightCycle.FixedDeltaTimeInHours);
		return ExecutorStatus.Running;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(WorkAtReservableExecutorKey);
		component.Set(FinishTimestampKey, _finishTimestamp);
		component.Set(IsLegacyExecutorKey, value: false);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(WorkAtReservableExecutorKey);
		_finishTimestamp = component.Get(FinishTimestampKey);
		_isLegacyExecutor = !component.Has(IsLegacyExecutorKey);
		_startWorkingPostLoad = true;
	}

	protected abstract void Initialize();

	protected abstract bool CanComplete();

	protected abstract void PerformActionOnTick(float deltaTime);

	protected abstract void PerformActionOnComplete();

	protected abstract void Unreserve();

	protected void Launch(float unscaledTimeInHours)
	{
		_finishTimestamp = CalculateFinishTimestamp(unscaledTimeInHours);
		StartWorking();
	}

	private void StartWorking()
	{
		TurnOnAnimation();
		WorkStarted?.Invoke(this, EventArgs.Empty);
	}

	private ExecutorStatus Stop()
	{
		WorkFinished?.Invoke(this, new WorkFinishedEventArgs(wasCompleted: false));
		TurnOffAnimation();
		Unreserve();
		return ExecutorStatus.Failure;
	}

	private ExecutorStatus Complete()
	{
		WorkFinished?.Invoke(this, new WorkFinishedEventArgs(wasCompleted: true));
		TurnOffAnimation();
		PerformActionOnComplete();
		return ExecutorStatus.Success;
	}

	private void TurnOnAnimation()
	{
		_animationTurnedOn = Animation;
		_characterAnimator.SetBool(_animationTurnedOn, value: true);
	}

	private void TurnOffAnimation()
	{
		if (!string.IsNullOrEmpty(_animationTurnedOn))
		{
			_characterAnimator.SetBool(_animationTurnedOn, value: false);
			_animationTurnedOn = null;
		}
	}

	private float CalculateFinishTimestamp(float unscaledTimeInHours)
	{
		float hours = unscaledTimeInHours / _worker.WorkingSpeedMultiplier;
		return _dayNightCycle.DayNumberHoursFromNow(hours);
	}
}
