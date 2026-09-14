namespace Timberborn.TimeSystem;

public interface ITickProgressService
{
	float Progress { get; }

	float SecondsPassedThisTick { get; }
}
