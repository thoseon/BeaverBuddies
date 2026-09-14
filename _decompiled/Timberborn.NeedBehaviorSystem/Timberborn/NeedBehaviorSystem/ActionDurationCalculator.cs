using Timberborn.BaseComponentSystem;
using Timberborn.NeedSystem;
using Timberborn.WalkingSystem;
using UnityEngine;

namespace Timberborn.NeedBehaviorSystem;

public class ActionDurationCalculator : BaseComponent, IAwakableComponent
{
	private Walker _walker;

	private NeedManager _needManager;

	public void Awake()
	{
		_walker = GetComponent<Walker>();
		_needManager = GetComponent<NeedManager>();
	}

	public float DurationWithReturnInHours(Vector3 actionPosition, Vector3 returnPosition)
	{
		return TravelTimeBetween(base.Transform.position, actionPosition) + TravelTimeBetween(actionPosition, returnPosition) + 0.3f;
	}

	public float FullyEffectiveDurationInHours(in EssentialAction essentialAction)
	{
		return TravelTimeBetween(base.Transform.position, essentialAction.Position) + essentialAction.MinDurationInHours + _needManager.FullyEffectiveDurationInHours(essentialAction.Effect);
	}

	private float TravelTimeBetween(Vector3 actionPosition, Vector3 position)
	{
		return _walker.CalculateTravelTimeInHours(actionPosition, position);
	}
}
