using Timberborn.InputSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Debugging;

public class DevModeController : IPriorityInputProcessor, ILoadableSingleton
{
	private static readonly string ToggleDevModeKey = "ToggleDevMode";

	private readonly DevModeManager _devModeManager;

	private readonly InputService _inputService;

	public DevModeController(DevModeManager devModeManager, InputService inputService)
	{
		_devModeManager = devModeManager;
		_inputService = inputService;
	}

	public void Load()
	{
		_inputService.AddInputProcessor(this);
	}

	public void ProcessInput()
	{
		if (_inputService.IsKeyDown(ToggleDevModeKey))
		{
			Toggle();
		}
	}

	private void Toggle()
	{
		if (_devModeManager.Enabled)
		{
			_devModeManager.Disable();
		}
		else
		{
			_devModeManager.Enable();
		}
	}
}
