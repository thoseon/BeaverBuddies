using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;

namespace Timberborn.NeedBehaviorSystem;

public class CriticalNeederRootBehavior : RootBehavior, IAwakableComponent
{
	private INeedBehaviorPicker _needBehaviorPicker;

	private BehaviorManager _behaviorManager;

	private BehaviorAgent _behaviorAgent;

	private bool _needsInCriticalStateOnly;

	public bool NeedRunning
	{
		get
		{
			if (!_behaviorManager.IsRunningBehavior<NeedBehavior>())
			{
				return _behaviorManager.IsRunningBehavior<EssentialNeedBehavior>();
			}
			return true;
		}
	}

	public void Awake()
	{
		_needBehaviorPicker = GetComponent<INeedBehaviorPicker>();
		_behaviorManager = GetComponent<BehaviorManager>();
		_behaviorAgent = GetComponent<BehaviorAgent>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		Behavior bestNeedBehaviorAffectingNeedsInCriticalState = _needBehaviorPicker.GetBestNeedBehaviorAffectingNeedsInCriticalState();
		if (bestNeedBehaviorAffectingNeedsInCriticalState != null)
		{
			Decision decision = bestNeedBehaviorAffectingNeedsInCriticalState.Decide(_behaviorAgent);
			if (!decision.ShouldReleaseNow)
			{
				return Decision.TransferNow(bestNeedBehaviorAffectingNeedsInCriticalState, in decision);
			}
		}
		return Decision.ReleaseNow();
	}
}
