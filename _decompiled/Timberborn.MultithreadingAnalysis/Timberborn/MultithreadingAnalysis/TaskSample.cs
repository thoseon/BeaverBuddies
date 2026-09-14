using System;
using System.Linq;
using System.Threading;

namespace Timberborn.MultithreadingAnalysis;

public readonly struct TaskSample
{
	private readonly Type _type;

	public int Run { get; }

	public int TotalRuns { get; }

	public long StartTime { get; }

	public long EndTime { get; }

	public Thread Thread { get; }

	public Type GenericType => _type.GenericTypeArguments.First();

	public TaskSample(int run, int totalRuns, long startTime, long endTime, Thread thread, Type type)
	{
		Run = run;
		TotalRuns = totalRuns;
		StartTime = startTime;
		EndTime = endTime;
		Thread = thread;
		_type = type;
	}
}
