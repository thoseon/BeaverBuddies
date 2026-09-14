using Timberborn.KeyBindingSystem;

namespace Timberborn.Debugging;

internal class DevModeKeyBindingBlocker : IKeyBindingBlocker
{
	private readonly DevModeManager _devModeManager;

	public DevModeKeyBindingBlocker(DevModeManager devModeManager)
	{
		_devModeManager = devModeManager;
	}

	public bool IsKeyBlocked(KeyBinding keyBinding)
	{
		if (keyBinding.DevModeOnly)
		{
			return !_devModeManager.Enabled;
		}
		return false;
	}
}
