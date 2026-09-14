using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.EntitySystem;
using Timberborn.NeedBehaviorSystem;
using Timberborn.TickSystem;

namespace Timberborn.SleepSystem;

public class SleepSoundEmitter : TickableComponent, IAwakableComponent, IDeletableEntity
{
	private readonly SleepSoundController _sleepSoundController;

	private BehaviorManager _behaviorManager;

	private bool _wasSleeping;

	public SleepSoundEmitter(SleepSoundController sleepSoundController)
	{
		_sleepSoundController = sleepSoundController;
	}

	public void Awake()
	{
		_behaviorManager = GetComponent<BehaviorManager>();
	}

	public void DeleteEntity()
	{
		if (_wasSleeping)
		{
			_sleepSoundController.RemoveSleepingBeaver(this);
		}
	}

	public override void Tick()
	{
		bool flag = _behaviorManager.IsRunningBehavior<SleepNeedBehavior>() && _behaviorManager.IsRunningExecutor<ApplyEffectExecutor>();
		if (_wasSleeping && !flag)
		{
			_sleepSoundController.RemoveSleepingBeaver(this);
		}
		if (!_wasSleeping & flag)
		{
			_sleepSoundController.AddSleepingBeaver(this);
		}
		_wasSleeping = flag;
	}
}
