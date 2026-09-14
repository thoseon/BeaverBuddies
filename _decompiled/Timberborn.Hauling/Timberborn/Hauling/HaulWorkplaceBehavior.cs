using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.WorkSystem;

namespace Timberborn.Hauling;

internal class HaulWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private HaulingCenter _haulingCenter;

	private readonly List<WorkplaceBehavior> _workplaceBehaviors = new List<WorkplaceBehavior>();

	public void Awake()
	{
		_haulingCenter = GetComponent<HaulingCenter>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		_haulingCenter.GetWorkplaceBehaviorsOrdered(_workplaceBehaviors);
		foreach (WorkplaceBehavior workplaceBehavior in _workplaceBehaviors)
		{
			Decision decision = workplaceBehavior.Decide(agent);
			if (!decision.ShouldReleaseNow)
			{
				_workplaceBehaviors.Clear();
				return Decision.TransferNow(workplaceBehavior, in decision);
			}
		}
		_workplaceBehaviors.Clear();
		return Decision.ReleaseNow();
	}
}
