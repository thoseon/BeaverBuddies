using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.DwellingSystem;
using Timberborn.Effects;
using Timberborn.EnterableSystem;
using Timberborn.GameDistricts;
using Timberborn.Navigation;
using Timberborn.NeedBehaviorSystem;
using Timberborn.NeedSpecs;
using Timberborn.Persistence;
using Timberborn.WalkingSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.SleepSystem;

public class SleepNeedBehavior : EssentialNeedBehavior, IAwakableComponent, IPersistentEntity
{
	private static readonly ComponentKey SleepNeedBehaviorKey = new ComponentKey("SleepNeedBehavior");

	private static readonly PropertyKey<bool> WalkedToSleepingPositionKey = new PropertyKey<bool>("WalkedToSleepingPosition");

	private static readonly float MaxDistanceToHomeWhenSleepingCritically = 13f;

	private readonly RandomDestinationPicker _randomDestinationPicker;

	private readonly INavigationService _navigationService;

	private Dweller _dweller;

	private Sleeper _sleeper;

	private Citizen _citizen;

	private WalkToPositionExecutor _walkToPositionExecutor;

	private WalkInsideExecutor _walkInsideExecutor;

	private ImmutableArray<ContinuousEffect> _actionEffects;

	private bool _walkedToSleepingPosition;

	public SleepNeedBehavior(RandomDestinationPicker randomDestinationPicker, INavigationService navigationService)
	{
		_randomDestinationPicker = randomDestinationPicker;
		_navigationService = navigationService;
	}

	public void Awake()
	{
		_dweller = GetComponent<Dweller>();
		_sleeper = GetComponent<Sleeper>();
		_citizen = GetComponent<Citizen>();
		_walkToPositionExecutor = GetComponent<WalkToPositionExecutor>();
		_walkInsideExecutor = GetComponent<WalkInsideExecutor>();
	}

	public override EssentialAction GetEssentialAction()
	{
		if (ShouldSleepAtHome())
		{
			Vector3? homeAccess = _dweller.HomeAccess;
			if (homeAccess.HasValue)
			{
				Vector3 valueOrDefault = homeAccess.GetValueOrDefault();
				return SleepAtHomeAction(valueOrDefault);
			}
		}
		return SleepOutsideAction();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (!ShouldSleepAtHome())
		{
			return SleepOutside();
		}
		return SleepAtHome();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_walkedToSleepingPosition)
		{
			entitySaver.GetComponent(SleepNeedBehaviorKey).Set(WalkedToSleepingPositionKey, _walkedToSleepingPosition);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(SleepNeedBehaviorKey, out var objectLoader))
		{
			_walkedToSleepingPosition = objectLoader.Get(WalkedToSleepingPositionKey);
		}
	}

	private bool ShouldSleepAtHome()
	{
		if (_dweller.HasHome)
		{
			if (_sleeper.ShouldSleepCritically())
			{
				return IsCloseToHome();
			}
			return true;
		}
		return false;
	}

	private bool IsCloseToHome()
	{
		Vector3? homeAccess = _dweller.HomeAccess;
		if (homeAccess.HasValue)
		{
			Vector3 valueOrDefault = homeAccess.GetValueOrDefault();
			return _navigationService.HeuristicDistance(base.Transform.position, valueOrDefault) < MaxDistanceToHomeWhenSleepingCritically;
		}
		return false;
	}

	private EssentialAction SleepAtHomeAction(Vector3 homeAccess)
	{
		IReadOnlyCollection<ContinuousEffectSpec> sleepEffects = _dweller.Home.SleepEffects;
		return SleepAction(homeAccess, sleepEffects);
	}

	private EssentialAction SleepOutsideAction()
	{
		ImmutableArray<ContinuousEffectSpec> sleepOutsideEffects = _sleeper.SleepOutsideEffects;
		return SleepAction(base.Transform.position, sleepOutsideEffects);
	}

	private EssentialAction SleepAction(Vector3 position, IEnumerable<ContinuousEffectSpec> effectSpecs)
	{
		ContinuousEffect effect = ContinuousEffect.FromSpec(effectSpecs.Single((ContinuousEffectSpec continuousEffectSpec) => continuousEffectSpec.NeedId == Sleeper.SleepNeedId));
		float minDurationInHours = (_sleeper.IsNewborn ? 24f : 0f);
		return new EssentialAction(position, effect, minDurationInHours);
	}

	private Decision SleepAtHome()
	{
		Dwelling home = _dweller.Home;
		return _walkInsideExecutor.Launch(home.GetComponent<Enterable>()) switch
		{
			ExecutorStatus.Success => Sleep(home.SleepEffects), 
			ExecutorStatus.Failure => UnassignHome(), 
			ExecutorStatus.Running => Decision.ReturnWhenFinished(_walkInsideExecutor), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private Decision UnassignHome()
	{
		_dweller.UnassignFromHome();
		return Decision.ReleaseNextTick();
	}

	private Decision SleepOutside()
	{
		if (!_walkedToSleepingPosition)
		{
			return WalkToRandomSleepingPosition();
		}
		return Sleep(_sleeper.SleepOutsideEffects);
	}

	private Decision WalkToRandomSleepingPosition()
	{
		_walkedToSleepingPosition = true;
		if (_randomDestinationPicker.TryGetSafeRandomDestination(_citizen, out var destination))
		{
			switch (_walkToPositionExecutor.Launch(destination))
			{
			case ExecutorStatus.Success:
			case ExecutorStatus.Failure:
				return Decision.ReturnNextTick();
			case ExecutorStatus.Running:
				return Decision.ReturnWhenFinished(_walkToPositionExecutor);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return Decision.ReturnNextTick();
	}

	private Decision Sleep(IEnumerable<ContinuousEffectSpec> sleepEffectsSpecs)
	{
		_walkedToSleepingPosition = false;
		return Decision.ReleaseWhenFinished(_sleeper.LaunchExecutor(sleepEffectsSpecs));
	}
}
