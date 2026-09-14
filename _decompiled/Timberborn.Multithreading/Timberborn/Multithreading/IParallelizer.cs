using System;

namespace Timberborn.Multithreading;

public interface IParallelizer
{
	long LastTaskTimestamp { get; }

	int NumberOfThreads { get; }

	ParallelizerHandle Schedule<T>(in T task) where T : struct, IParallelizerSingleTask;

	ParallelizerHandle Schedule<T>(in T task, ParallelizerHandle dependency) where T : struct, IParallelizerSingleTask;

	ParallelizerHandle Schedule<T>(in T task, ReadOnlySpan<ParallelizerHandle> dependencies) where T : struct, IParallelizerSingleTask;

	ParallelizerHandle Schedule<T>(int fromInclusive, int toExclusive, int batchSize, in T task) where T : struct, IParallelizerLoopTask;

	ParallelizerHandle Schedule<T>(int fromInclusive, int toExclusive, int batchSize, in T task, ParallelizerHandle dependency) where T : struct, IParallelizerLoopTask;

	ParallelizerHandle Schedule<T>(int fromInclusive, int toExclusive, int batchSize, in T task, ReadOnlySpan<ParallelizerHandle> dependencies) where T : struct, IParallelizerLoopTask;

	void StartScheduling();

	void StopScheduling();

	void Wait();

	void ThrowIfAnyPendingTasks();
}
