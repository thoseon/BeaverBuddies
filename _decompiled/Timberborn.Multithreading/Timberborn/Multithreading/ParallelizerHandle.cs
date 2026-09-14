namespace Timberborn.Multithreading;

public readonly struct ParallelizerHandle
{
	private readonly Parallelizer _parallelizer;

	internal IScheduledTask Task { get; }

	internal int Version { get; }

	internal ParallelizerHandle(IScheduledTask task, int version, Parallelizer parallelizer)
	{
		Task = task;
		Version = version;
		_parallelizer = parallelizer;
	}

	public ParallelizerHandle ContinueWith<T>(in T task) where T : struct, IParallelizerSingleTask
	{
		return _parallelizer.Schedule(in task, this);
	}

	public ParallelizerHandle ContinueWith<T>(int fromInclusive, int toExclusive, int batchSize, in T task) where T : struct, IParallelizerLoopTask
	{
		return _parallelizer.Schedule(fromInclusive, toExclusive, batchSize, in task, this);
	}
}
