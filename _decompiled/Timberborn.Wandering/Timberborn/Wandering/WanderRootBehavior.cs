using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Common;
using Timberborn.EnterableSystem;
using Timberborn.GameDistricts;
using Timberborn.Persistence;
using Timberborn.WalkingSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Wandering;

public class WanderRootBehavior : RootBehavior, IAwakableComponent, IPersistentEntity
{
	private static readonly ComponentKey WanderRootBehaviorKey = new ComponentKey("WanderRootBehavior");

	private static readonly PropertyKey<bool> WalkedKey = new PropertyKey<bool>("Walked");

	private static readonly float GoToRestPlaceChance = 0.2f;

	private readonly RandomDestinationPicker _randomDestinationPicker;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private Citizen _citizen;

	private WalkToPositionExecutor _walkToPositionExecutor;

	private WalkInsideExecutor _walkInsideExecutor;

	private WaitExecutor _waitExecutor;

	private bool _walked;

	private bool _restPlacesAllowed;

	public event EventHandler IdleStarted;

	public WanderRootBehavior(RandomDestinationPicker randomDestinationPicker, IRandomNumberGenerator randomNumberGenerator)
	{
		_randomDestinationPicker = randomDestinationPicker;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		_citizen = GetComponent<Citizen>();
		_walkToPositionExecutor = GetComponent<WalkToPositionExecutor>();
		_walkInsideExecutor = GetComponent<WalkInsideExecutor>();
		_waitExecutor = GetComponent<WaitExecutor>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (_walked)
		{
			float hours = _randomNumberGenerator.Range(0.1f, 0.3f);
			_waitExecutor.LaunchForSpecifiedTime(hours);
			_walked = false;
			IdleStarted?.Invoke(this, EventArgs.Empty);
			return Decision.ReleaseWhenFinished(_waitExecutor);
		}
		_walked = true;
		return WalkToRandomDestination();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_walked)
		{
			entitySaver.GetComponent(WanderRootBehaviorKey).Set(WalkedKey, _walked);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(WanderRootBehaviorKey, out var objectLoader))
		{
			_walked = objectLoader.Get(WalkedKey);
		}
	}

	public void AllowVisitingRestPlaces()
	{
		_restPlacesAllowed = true;
	}

	private Decision WalkToRandomDestination()
	{
		if (ShouldWalkToRandomRestPlace())
		{
			RestPlace restPlace = RandomRestPlaceInDistrict();
			if (restPlace != null)
			{
				return WalkToRestPlace(restPlace);
			}
		}
		return WanderAround();
	}

	private RestPlace RandomRestPlaceInDistrict()
	{
		IEnumerable<RestPlace> enabledBuildings = _citizen.AssignedDistrict.DistrictBuildingRegistry.GetEnabledBuildings<RestPlace>();
		if (!_randomNumberGenerator.TryGetEnumerableElement(enabledBuildings, out var randomElement))
		{
			return null;
		}
		return randomElement;
	}

	private bool ShouldWalkToRandomRestPlace()
	{
		if (_restPlacesAllowed)
		{
			return _randomNumberGenerator.Range(0f, 1f) < GoToRestPlaceChance;
		}
		return false;
	}

	private Decision WalkToRestPlace(RestPlace restPlace)
	{
		return _walkInsideExecutor.Launch(restPlace.GetComponent<Enterable>()) switch
		{
			ExecutorStatus.Success => Decision.ReleaseNow(), 
			ExecutorStatus.Running => Decision.ReturnWhenFinished(_walkInsideExecutor), 
			ExecutorStatus.Failure => Decision.ReleaseNow(), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private Decision WanderAround()
	{
		if (_randomDestinationPicker.TryGetSafeRandomDestination(_citizen, out var destination))
		{
			return _walkToPositionExecutor.Launch(destination) switch
			{
				ExecutorStatus.Success => Decision.ReleaseNow(), 
				ExecutorStatus.Failure => Decision.ReleaseNow(), 
				ExecutorStatus.Running => Decision.ReturnWhenFinished(_walkToPositionExecutor), 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
		return Decision.ReleaseNow();
	}
}
