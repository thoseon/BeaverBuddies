using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Common;
using Timberborn.EnterableSystem;
using Timberborn.GameDistricts;
using Timberborn.WalkingSystem;

namespace Timberborn.Wandering;

public class StrandedRootBehavior : RootBehavior, IAwakableComponent
{
	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly RandomDestinationPicker _randomDestinationPicker;

	private Citizen _citizen;

	private Enterer _enterer;

	private WalkToPositionExecutor _walkToPositionExecutor;

	private AnimateExecutor _animateExecutor;

	private float RandomWaitingTimeInHours => _randomNumberGenerator.Range(0.8f, 1.2f);

	public StrandedRootBehavior(IRandomNumberGenerator randomNumberGenerator, RandomDestinationPicker randomDestinationPicker)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_randomDestinationPicker = randomDestinationPicker;
	}

	public void Awake()
	{
		_citizen = GetComponent<Citizen>();
		_enterer = GetComponent<Enterer>();
		_walkToPositionExecutor = GetComponent<WalkToPositionExecutor>();
		_animateExecutor = GetComponent<AnimateExecutor>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (!_citizen.HasAssignedDistrict)
		{
			if (_enterer.IsInside && _randomDestinationPicker.TryGetSafeRandomDestination(_citizen, out var destination))
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
			_animateExecutor.Launch("Stranded", RandomWaitingTimeInHours);
			return Decision.ReleaseWhenFinished(_animateExecutor);
		}
		return Decision.ReleaseNow();
	}
}
