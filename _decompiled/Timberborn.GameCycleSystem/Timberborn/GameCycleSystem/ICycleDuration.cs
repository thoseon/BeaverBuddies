namespace Timberborn.GameCycleSystem;

public interface ICycleDuration
{
	int DurationInDays { get; }

	void SetForCycle(int cycle);
}
