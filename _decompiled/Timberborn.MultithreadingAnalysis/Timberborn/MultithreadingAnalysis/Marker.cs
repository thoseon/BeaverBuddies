using System.Threading;

namespace Timberborn.MultithreadingAnalysis;

public readonly struct Marker
{
	public string Id { get; }

	public long Timestamp { get; }

	public Thread Thread { get; }

	public Marker(string id, long timestamp, Thread thread)
	{
		Id = id;
		Timestamp = timestamp;
		Thread = thread;
	}
}
