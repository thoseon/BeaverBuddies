namespace Timberborn.GameCycleSystem;

public class CycleStartedEvent
{
	public int Cycle { get; }

	public CycleStartedEvent(int cycle)
	{
		Cycle = cycle;
	}
}
