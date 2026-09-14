using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Coordinates;
using Timberborn.WalkingSystem;
using UnityEngine;

namespace Timberborn.CharacterControlSystem;

public class CharacterControlRootBehavior : RootBehavior, IAwakableComponent
{
	private WalkToPositionExecutor _walkToPositionExecutor;

	private ControllableCharacter _controllableCharacter;

	public void Awake()
	{
		_controllableCharacter = GetComponent<ControllableCharacter>();
		_walkToPositionExecutor = GetComponent<WalkToPositionExecutor>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (_controllableCharacter.UnderControl)
		{
			Vector3 position = CoordinateSystem.GridToWorld(_controllableCharacter.Destination);
			switch (_walkToPositionExecutor.Launch(position))
			{
			case ExecutorStatus.Success:
				_controllableCharacter.PlayAnimation();
				return Decision.ReturnNextTick();
			case ExecutorStatus.Failure:
				return Decision.ReleaseNow();
			case ExecutorStatus.Running:
				return Decision.ReturnWhenFinished(_walkToPositionExecutor);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return Decision.ReleaseNow();
	}
}
