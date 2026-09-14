using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;

namespace Timberborn.NeedBehaviorSystem;

public class NeederRootBehavior : RootBehavior, IAwakableComponent
{
	private INeedBehaviorPicker _needBehaviorPicker;

	private BehaviorAgent _behaviorAgent;

	public void Awake()
	{
		_needBehaviorPicker = GetComponent<INeedBehaviorPicker>();
		_behaviorAgent = GetComponent<BehaviorAgent>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		Behavior bestNeedBehavior = _needBehaviorPicker.GetBestNeedBehavior();
		if (bestNeedBehavior != null)
		{
			Decision decision = bestNeedBehavior.Decide(_behaviorAgent);
			if (!decision.ShouldReleaseNow)
			{
				return Decision.TransferNow(bestNeedBehavior, in decision);
			}
		}
		return Decision.ReleaseNow();
	}
}
