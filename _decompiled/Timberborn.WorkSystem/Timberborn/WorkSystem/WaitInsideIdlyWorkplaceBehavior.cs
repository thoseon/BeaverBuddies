using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.EnterableSystem;
using Timberborn.WalkingSystem;

namespace Timberborn.WorkSystem;

public class WaitInsideIdlyWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private Workplace _workplace;

	private Enterable _enterable;

	public void Awake()
	{
		_workplace = GetComponent<Workplace>();
		_enterable = GetComponent<Enterable>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		WalkInsideExecutor component = agent.GetComponent<WalkInsideExecutor>();
		switch (component.LaunchForLimitedTime(_enterable))
		{
		case ExecutorStatus.Success:
		{
			WaitExecutor component3 = agent.GetComponent<WaitExecutor>();
			component3.LaunchForIdleTime();
			return Decision.ReleaseWhenFinished(component3);
		}
		case ExecutorStatus.Failure:
		{
			Worker component2 = agent.GetComponent<Worker>();
			_workplace.UnassignWorker(component2);
			return Decision.ReleaseNextTick();
		}
		case ExecutorStatus.Running:
			return Decision.ReleaseWhenFinished(component);
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
