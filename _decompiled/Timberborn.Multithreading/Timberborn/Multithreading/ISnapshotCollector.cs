using System;

namespace Timberborn.Multithreading;

public interface ISnapshotCollector
{
	bool IsCollecting { get; }

	void AddTaskSample(int run, int totalRuns, long startTimestamp, long endTimestamp, Type type);

	void AddMarker(string id);
}
