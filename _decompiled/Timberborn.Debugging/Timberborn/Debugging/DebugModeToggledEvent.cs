namespace Timberborn.Debugging;

public class DebugModeToggledEvent
{
	public bool Enabled { get; }

	public DebugModeToggledEvent(bool enabled)
	{
		Enabled = enabled;
	}
}
