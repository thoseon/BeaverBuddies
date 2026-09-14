using System;

namespace Timberborn.Multithreading;

internal readonly struct LoopTaskRunner<T> : ITaskRunner where T : struct, IParallelizerLoopTask
{
	private readonly T _task;

	private readonly int _fromInclusive;

	private readonly int _toExclusive;

	private readonly int _batchSize;

	public int ExpectedRuns { get; }

	public LoopTaskRunner(T task, int fromInclusive, int toExclusive, int batchSize)
	{
		_task = task;
		_fromInclusive = fromInclusive;
		_toExclusive = toExclusive;
		_batchSize = batchSize;
		ExpectedRuns = (_toExclusive - _fromInclusive + batchSize - 1) / batchSize;
	}

	public void Run(int runIndex)
	{
		int expectedRuns = ExpectedRuns;
		if (runIndex < 0)
		{
			throw new ArgumentException(string.Format("{0} {1} of task {2}", "runIndex", runIndex, typeof(T)) + " must be at least zero");
		}
		if (runIndex >= expectedRuns)
		{
			throw new ArgumentException(string.Format("{0} {1} of task {2} must be less than {3}", "runIndex", runIndex, typeof(T), expectedRuns));
		}
		int num = _fromInclusive + runIndex * _batchSize;
		int num2 = Math.Min(num + _batchSize, _toExclusive);
		for (int i = num; i < num2; i++)
		{
			_task.Run(i);
		}
	}
}
