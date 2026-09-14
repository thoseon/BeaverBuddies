using System.Diagnostics;

namespace Timberborn.MultithreadingAnalysisUI;

internal static class TaskSampleCalculator
{
	public static double TicksToMs(long ticks)
	{
		return (double)ticks * 1000.0 / (double)Stopwatch.Frequency;
	}
}
