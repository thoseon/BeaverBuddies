namespace Timberborn.BehaviorSystem;

public readonly struct Decision
{
	public IExecutor Executor { get; }

	public Behavior Behavior { get; }

	public bool ShouldReleaseNow { get; }

	public bool ShouldReturnToBehavior { get; }

	private Decision(IExecutor executor, Behavior behavior, bool shouldReleaseNow, bool shouldReturnToBehavior)
	{
		Executor = executor;
		Behavior = behavior;
		ShouldReleaseNow = shouldReleaseNow;
		ShouldReturnToBehavior = shouldReturnToBehavior;
	}

	public static Decision ReturnWhenFinished(IExecutor executor)
	{
		return new Decision(executor, null, shouldReleaseNow: false, shouldReturnToBehavior: true);
	}

	public static Decision ReleaseWhenFinished(IExecutor executor)
	{
		return new Decision(executor, null, shouldReleaseNow: false, shouldReturnToBehavior: false);
	}

	public static Decision ReturnNextTick()
	{
		return new Decision(null, null, shouldReleaseNow: false, shouldReturnToBehavior: true);
	}

	public static Decision ReleaseNextTick()
	{
		return new Decision(null, null, shouldReleaseNow: false, shouldReturnToBehavior: false);
	}

	public static Decision TransferNow(Behavior behavior, in Decision decision)
	{
		Behavior behavior2 = decision.Behavior ?? behavior;
		return new Decision(decision.Executor, behavior2, decision.ShouldReleaseNow, decision.ShouldReturnToBehavior);
	}

	public static Decision ReleaseNow()
	{
		return new Decision(null, null, shouldReleaseNow: true, shouldReturnToBehavior: false);
	}
}
