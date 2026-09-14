using System;

namespace Timberborn.Multithreading;

internal readonly struct SingleTaskRunner<T>(T task) : ITaskRunner where T : struct, IParallelizerSingleTask
{
	private readonly T _task = task;

	public int ExpectedRuns => 1;

	public void Run(int runIndex)
	{
		if (runIndex != 0)
		{
			throw new ArgumentException("runIndex must be zero");
		}
		_task.Run();
	}
}
