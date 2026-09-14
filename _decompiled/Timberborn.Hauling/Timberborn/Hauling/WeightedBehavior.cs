using Timberborn.WorkSystem;

namespace Timberborn.Hauling;

public readonly struct WeightedBehavior
{
	public float Weight { get; }

	public WorkplaceBehavior WorkplaceBehavior { get; }

	public WeightedBehavior(float weight, WorkplaceBehavior workplaceBehavior)
	{
		Weight = weight;
		WorkplaceBehavior = workplaceBehavior;
	}
}
