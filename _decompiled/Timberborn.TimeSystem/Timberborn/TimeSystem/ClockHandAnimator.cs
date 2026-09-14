using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.TimeSystem;

internal class ClockHandAnimator : TickableComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly IDayNightCycle _dayNightCycle;

	private ClockHandAnimatorSpec _clockHandAnimatorSpec;

	private Transform _hand;

	private Vector3 _initialRotation;

	public ClockHandAnimator(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_clockHandAnimatorSpec = GetComponent<ClockHandAnimatorSpec>();
		_hand = base.GameObject.FindChildTransform(_clockHandAnimatorSpec.HandName);
		_initialRotation = _hand.localRotation.eulerAngles;
		DisableComponent();
	}

	public override void Tick()
	{
		UpdateHandRotation();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		UpdateHandRotation();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	private void UpdateHandRotation()
	{
		float z = (0f - _dayNightCycle.DayProgress) * 360f + _clockHandAnimatorSpec.AngleOffset;
		_hand.localRotation = Quaternion.Euler(new Vector3(0f, 0f, z) + _initialRotation);
	}
}
