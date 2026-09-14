using Timberborn.Common;

namespace Timberborn.MultithreadingAnalysis;

public class Snapshot
{
	public int Ticks { get; }

	public ReadOnlyList<TaskSample> TaskSamples { get; }

	public ReadOnlyList<Marker> Markers { get; }

	public Snapshot(int ticks, ReadOnlyList<TaskSample> taskSamples, ReadOnlyList<Marker> markers)
	{
		Ticks = ticks;
		TaskSamples = taskSamples;
		Markers = markers;
	}
}
