using Timberborn.Common;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.TimeSystem;

public class SpeedManager : IPostLoadableSingleton, ILateUpdatableSingleton
{
	private readonly EventBus _eventBus;

	private float _currentSpeedScale = 1f;

	private float? _nextSpeed;

	private float _speedBefore;

	private bool _isLocked;

	public float CurrentSpeed { get; private set; }

	public SpeedManager(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void PostLoad()
	{
		_eventBus.Post(new CurrentSpeedChangedEvent(CurrentSpeed));
	}

	public void LateUpdateSingleton()
	{
		ChangeSpeed();
	}

	public void ChangeSpeedScale(float speedScale)
	{
		Asserts.ValueIsInRange(speedScale, 0f, 1f, "speedScale");
		_currentSpeedScale = speedScale;
		float valueOrDefault = _nextSpeed.GetValueOrDefault();
		if (!_nextSpeed.HasValue)
		{
			valueOrDefault = CurrentSpeed;
			_nextSpeed = valueOrDefault;
		}
	}

	public void ChangeSpeed(float speed)
	{
		if (!_isLocked)
		{
			_nextSpeed = speed;
		}
	}

	public void ChangeAndLockSpeed(float value)
	{
		if (!_isLocked)
		{
			_speedBefore = CurrentSpeed;
			ChangeSpeed(value);
			_isLocked = true;
			_eventBus.Post(new SpeedLockChangedEvent(_isLocked));
		}
	}

	public void UnlockSpeed()
	{
		if (_isLocked)
		{
			_isLocked = false;
			ChangeSpeed(_speedBefore);
			_eventBus.Post(new SpeedLockChangedEvent(_isLocked));
		}
	}

	private void ChangeSpeed()
	{
		if (_nextSpeed.HasValue)
		{
			float value = _nextSpeed.Value;
			Time.timeScale = ScaleSpeed(value);
			CurrentSpeed = value;
			_nextSpeed = null;
			_eventBus.Post(new CurrentSpeedChangedEvent(CurrentSpeed));
		}
	}

	private float ScaleSpeed(float speed)
	{
		if (!(speed > 1f))
		{
			return speed;
		}
		return Mathf.Lerp(1f, speed, _currentSpeedScale);
	}
}
