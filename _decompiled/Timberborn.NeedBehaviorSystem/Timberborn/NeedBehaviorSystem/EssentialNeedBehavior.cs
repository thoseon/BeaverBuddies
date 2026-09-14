using Timberborn.BehaviorSystem;

namespace Timberborn.NeedBehaviorSystem;

public abstract class EssentialNeedBehavior : Behavior
{
	public abstract EssentialAction GetEssentialAction();
}
