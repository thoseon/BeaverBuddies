namespace Timberborn.GameCycleSystem;

public class CycleEndedEvent
{
	public int Cycle { get; }

	public CycleEndedEvent(int cycle)
	{
		Cycle = cycle;
	}
}
