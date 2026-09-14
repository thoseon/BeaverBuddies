namespace Timberborn.BehaviorSystem;

public static class ExecutorExtensions
{
	public static string GetName(this IExecutor executor)
	{
		return executor.GetType().Name;
	}
}
