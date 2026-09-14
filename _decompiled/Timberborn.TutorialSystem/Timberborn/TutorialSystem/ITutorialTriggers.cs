namespace Timberborn.TutorialSystem;

public interface ITutorialTriggers
{
	void AddTrigger(string triggerId);

	bool TriggerPending(string triggerId);
}
