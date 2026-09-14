using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Persistence;
using Timberborn.TimbermeshAnimations;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.CharacterModelSystem;

public class AnimateExecutor : BaseComponent, IAwakableComponent, IExecutor
{
	private static readonly ComponentKey AnimateExecutorKey = new ComponentKey("AnimateExecutor");

	private static readonly PropertyKey<string> AnimationNameKey = new PropertyKey<string>("AnimationName");

	private static readonly PropertyKey<float> FinishTimestampKey = new PropertyKey<float>("FinishTimestamp");

	private readonly IDayNightCycle _dayNightCycle;

	private CharacterAnimator _characterAnimator;

	private IAnimator _animator;

	private string _animationName;

	private float _finishTimestamp;

	private float _turnedOffTime;

	public AnimateExecutor(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_characterAnimator = GetComponent<CharacterAnimator>();
		_animator = GetComponentInChildren<IAnimator>();
	}

	public void Launch(string animationName, float hours)
	{
		bool continuation = _animationName == animationName;
		_animationName = animationName;
		_finishTimestamp = _dayNightCycle.DayNumberHoursFromNow(hours);
		TurnOnAnimation(continuation);
	}

	public ExecutorStatus Tick(float deltaTimeInHours)
	{
		if (_dayNightCycle.PartialDayNumber > _finishTimestamp)
		{
			TurnOffAnimation();
			return ExecutorStatus.Success;
		}
		return ExecutorStatus.Running;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(AnimateExecutorKey);
		component.Set(AnimationNameKey, _animationName);
		component.Set(FinishTimestampKey, _finishTimestamp);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(AnimateExecutorKey);
		_animationName = component.Get(AnimationNameKey);
		_finishTimestamp = component.Get(FinishTimestampKey);
		TurnOnAnimation();
	}

	private void TurnOnAnimation(bool continuation = false)
	{
		_characterAnimator.SetBool(_animationName, value: true);
		if (continuation)
		{
			_animator.SetTime(_turnedOffTime);
		}
		_turnedOffTime = 0f;
	}

	private void TurnOffAnimation()
	{
		_turnedOffTime = _animator.Time;
		_characterAnimator.SetBool(_animationName, value: false);
	}
}
