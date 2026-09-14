using Timberborn.Effects;
using UnityEngine;

namespace Timberborn.NeedBehaviorSystem;

public readonly struct EssentialAction
{
	public Vector3 Position { get; }

	public ContinuousEffect Effect { get; }

	public float MinDurationInHours { get; }

	public EssentialAction(Vector3 position, ContinuousEffect effect, float minDurationInHours)
	{
		Position = position;
		Effect = effect;
		MinDurationInHours = minDurationInHours;
	}
}
