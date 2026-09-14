using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.EnterableSystem;
using Timberborn.GameDistricts;
using Timberborn.MortalSystem;
using Timberborn.Persistence;
using Timberborn.WalkingSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.DeathSystem;

public class DieRootBehavior : RootBehavior, IAwakableComponent, IPersistentEntity
{
	private static readonly ComponentKey MortalRootBehaviorKey = new ComponentKey("MortalRootBehavior");

	private static readonly PropertyKey<bool> WentToDeathPositionKey = new PropertyKey<bool>("WentToDeathPosition");

	private readonly RandomDestinationPicker _randomDestinationPicker;

	private Mortal _mortal;

	private WalkToPositionExecutor _walkToPositionExecutor;

	private Enterer _enterer;

	private Citizen _citizen;

	private bool _wentToDeathPosition;

	private bool WalkToDeathPositionBeforeDying
	{
		get
		{
			if (!_mortal.ShouldDiePublicly)
			{
				return !_enterer.IsInside;
			}
			return true;
		}
	}

	public DieRootBehavior(RandomDestinationPicker randomDestinationPicker)
	{
		_randomDestinationPicker = randomDestinationPicker;
	}

	public void Awake()
	{
		_mortal = GetComponent<Mortal>();
		_enterer = GetComponent<Enterer>();
		_citizen = GetComponent<Citizen>();
		_walkToPositionExecutor = GetComponent<WalkToPositionExecutor>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (_mortal.IsTimeToDie)
		{
			if (WalkToDeathPositionBeforeDying && !_wentToDeathPosition)
			{
				_wentToDeathPosition = true;
				return GoToRandomDeathPosition();
			}
			_mortal.DieIfItIsTime();
			return Decision.ReleaseNextTick();
		}
		return Decision.ReleaseNow();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_wentToDeathPosition)
		{
			entitySaver.GetComponent(MortalRootBehaviorKey).Set(WentToDeathPositionKey, _wentToDeathPosition);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(MortalRootBehaviorKey, out var objectLoader))
		{
			_wentToDeathPosition = objectLoader.Get(WentToDeathPositionKey);
		}
	}

	private Decision GoToRandomDeathPosition()
	{
		Vector3 position = _randomDestinationPicker.RandomDestination(_citizen);
		switch (_walkToPositionExecutor.Launch(position))
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
}
