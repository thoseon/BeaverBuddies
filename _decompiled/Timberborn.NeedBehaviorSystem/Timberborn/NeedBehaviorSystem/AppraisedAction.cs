using System.Collections.Immutable;
using Timberborn.BehaviorSystem;

namespace Timberborn.NeedBehaviorSystem;

public readonly struct AppraisedAction
{
	public Behavior NeedBehavior { get; }

	public ImmutableArray<string> AffectedNeeds { get; }

	public float Points { get; }

	public AppraisedAction(Behavior needBehavior, ImmutableArray<string> affectedNeeds, float points)
	{
		NeedBehavior = needBehavior;
		AffectedNeeds = affectedNeeds;
		Points = points;
	}
}
