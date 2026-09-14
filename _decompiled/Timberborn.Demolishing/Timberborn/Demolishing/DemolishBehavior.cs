using System;
using Timberborn.BehaviorSystem;
using Timberborn.ReservableSystem;
using Timberborn.WorkSystem;

namespace Timberborn.Demolishing;

public class DemolishBehavior : Behavior, IJobBehavior
{
	public override Decision Decide(BehaviorAgent agent)
	{
		Demolisher component = agent.GetComponent<Demolisher>();
		if (!component.HasReservedDemolishable)
		{
			return Decision.ReleaseNow();
		}
		if (!component.ReservedDemolishable.CanBeDemolished)
		{
			return UnreserveDemolishable(component);
		}
		Demolishable demolishable = component.Demolishable;
		WalkToReservableExecutor component2 = agent.GetComponent<WalkToReservableExecutor>();
		DemolishableReacher component3 = demolishable.GetComponent<DemolishableReacher>();
		return component2.Launch(component3) switch
		{
			ExecutorStatus.Success => Demolish(agent), 
			ExecutorStatus.Failure => UnreserveDemolishable(component), 
			ExecutorStatus.Running => Decision.ReturnWhenFinished(component2), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private static Decision Demolish(BehaviorAgent agent)
	{
		DemolishExecutor component = agent.GetComponent<DemolishExecutor>();
		if (!component.Demolish())
		{
			return Decision.ReturnNextTick();
		}
		return Decision.ReleaseWhenFinished(component);
	}

	private static Decision UnreserveDemolishable(Demolisher demolisher)
	{
		demolisher.Unreserve();
		return Decision.ReleaseNextTick();
	}
}
