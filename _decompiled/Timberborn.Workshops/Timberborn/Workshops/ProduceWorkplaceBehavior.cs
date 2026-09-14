using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.EnterableSystem;
using Timberborn.WalkingSystem;
using Timberborn.WorkSystem;

namespace Timberborn.Workshops;

public class ProduceWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private Manufactory _manufactory;

	private Workplace _workplace;

	private Enterable _enterable;

	public void Awake()
	{
		_manufactory = GetComponent<Manufactory>();
		_workplace = GetComponent<Workplace>();
		_enterable = GetComponent<Enterable>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (_manufactory.IsReadyToProduce)
		{
			WalkInsideExecutor component = agent.GetComponent<WalkInsideExecutor>();
			switch (component.Launch(_enterable))
			{
			case ExecutorStatus.Success:
			{
				ProduceExecutor component3 = agent.GetComponent<ProduceExecutor>();
				if (!component3.Launch(0.25f))
				{
					return Decision.ReleaseNextTick();
				}
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
		return Decision.ReleaseNow();
	}
}
