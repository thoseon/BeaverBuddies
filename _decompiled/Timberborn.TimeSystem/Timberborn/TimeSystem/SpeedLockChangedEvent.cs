namespace Timberborn.TimeSystem;

public class SpeedLockChangedEvent
{
	public bool IsLocked { get; }

	public SpeedLockChangedEvent(bool isLocked)
	{
		IsLocked = isLocked;
	}
}
