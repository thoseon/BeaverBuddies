using Timberborn.BehaviorSystem;

namespace Timberborn.NeedBehaviorSystem;

public interface INeedBehaviorPicker
{
	Behavior GetBestNeedBehaviorAffectingNeedsInCriticalState();

	Behavior GetBestNeedBehavior();

	bool NeedIsBeingCriticallySatisfied(string needId);
}
