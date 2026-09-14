using System;

namespace Timberborn.SaveMetadataSystem;

public class SaveMetadata
{
	public DateTime Timestamp { get; }

	public int Cycle { get; }

	public int Day { get; }

	public ModReference[] Mods { get; }

	public SaveMetadata(DateTime timestamp, int cycle, int day, ModReference[] mods)
	{
		Timestamp = timestamp;
		Cycle = cycle;
		Day = day;
		Mods = mods;
	}
}
