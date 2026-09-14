namespace Timberborn.TimeSystem;

public interface ITimeTrigger
{
	float DaysLeft { get; }

	float Progress { get; }

	bool Finished { get; }

	bool InProgress { get; }

	void Reset();

	void Resume();

	void Pause();

	void FastForwardProgress(float progress);
}
