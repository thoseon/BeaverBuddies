namespace Timberborn.LevelVisibilitySystem;

public class MaxVisibleLevelChangedEvent
{
	public int OldMaxVisibleLevel { get; }

	public MaxVisibleLevelChangedEvent(int oldMaxVisibleLevel)
	{
		OldMaxVisibleLevel = oldMaxVisibleLevel;
	}
}
