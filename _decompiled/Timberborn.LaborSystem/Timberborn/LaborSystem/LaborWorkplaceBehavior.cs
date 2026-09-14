using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockSystem;
using Timberborn.WorkSystem;

namespace Timberborn.LaborSystem;

public class LaborWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent, IFinishedStateListener
{
	private readonly List<LaborBehavior> _laborBehaviors = new List<LaborBehavior>();

	public void Awake()
	{
		GetComponents(_laborBehaviors);
		DisableComponent();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		foreach (LaborBehavior laborBehavior in _laborBehaviors)
		{
			Decision decision = laborBehavior.Decide(agent);
			if (!decision.ShouldReleaseNow)
			{
				return Decision.TransferNow(laborBehavior, in decision);
			}
		}
		return Decision.ReleaseNow();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}
}
