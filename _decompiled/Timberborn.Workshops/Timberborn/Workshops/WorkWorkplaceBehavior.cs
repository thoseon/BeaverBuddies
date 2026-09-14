using Timberborn.BehaviorSystem;
using Timberborn.WorkSystem;

namespace Timberborn.Workshops;

public class WorkWorkplaceBehavior : WorkplaceBehavior
{
	public override Decision Decide(BehaviorAgent agent)
	{
		return agent.GetComponent<WorkplaceWorkStarter>().StartWorking();
	}
}
