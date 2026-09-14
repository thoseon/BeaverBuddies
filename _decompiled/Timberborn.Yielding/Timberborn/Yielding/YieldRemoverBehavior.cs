using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.InventorySystem;
using Timberborn.ReservableSystem;
using Timberborn.WorkSystem;

namespace Timberborn.Yielding;

public class YieldRemoverBehavior : Behavior, IAwakableComponent, IJobBehavior
{
	private YielderRemover _yielderRemover;

	private GoodReserver _goodReserver;

	private WalkToReservableExecutor _walkToReservableExecutor;

	private RemoveYieldExecutor _removeYieldExecutor;

	public void Awake()
	{
		_yielderRemover = GetComponent<YielderRemover>();
		_goodReserver = GetComponent<GoodReserver>();
		_walkToReservableExecutor = GetComponent<WalkToReservableExecutor>();
		_removeYieldExecutor = GetComponent<RemoveYieldExecutor>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		Yielder reservedYielder = _yielderRemover.ReservedYielder;
		if (!reservedYielder || !reservedYielder.RemoveYieldStrategy.IsStillRemovable)
		{
			return UnreserveYielder();
		}
		return _walkToReservableExecutor.Launch(reservedYielder.RemoveYieldStrategy.Reacher) switch
		{
			ExecutorStatus.Success => RemoveYielder(), 
			ExecutorStatus.Failure => UnreserveYielder(), 
			ExecutorStatus.Running => Decision.ReturnWhenFinished(_walkToReservableExecutor), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private Decision RemoveYielder()
	{
		if (!_removeYieldExecutor.Remove())
		{
			return Decision.ReturnNextTick();
		}
		return Decision.ReleaseWhenFinished(_removeYieldExecutor);
	}

	private Decision UnreserveYielder()
	{
		_goodReserver.UnreserveCapacity();
		_yielderRemover.Unreserve();
		return Decision.ReleaseNextTick();
	}
}
