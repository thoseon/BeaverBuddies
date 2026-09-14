namespace Timberborn.Debugging;

public class DevModeToggledEvent
{
	public bool Enabled { get; }

	public DevModeToggledEvent(bool enabled)
	{
		Enabled = enabled;
	}
}
