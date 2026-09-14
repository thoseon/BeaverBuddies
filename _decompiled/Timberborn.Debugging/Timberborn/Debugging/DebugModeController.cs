using Timberborn.InputSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Debugging;

public class DebugModeController : IPriorityInputProcessor, ILoadableSingleton
{
	private static readonly string ToggleDebugModeKey = "ToggleDebugMode";

	private readonly DebugModeManager _debugModeManager;

	private readonly InputService _inputService;

	public DebugModeController(DebugModeManager debugModeManager, InputService inputService)
	{
		_debugModeManager = debugModeManager;
		_inputService = inputService;
	}

	public void Load()
	{
		_inputService.AddInputProcessor(this);
	}

	public void ProcessInput()
	{
		if (_inputService.IsKeyDown(ToggleDebugModeKey))
		{
			Toggle();
		}
	}

	private void Toggle()
	{
		if (_debugModeManager.Enabled)
		{
			_debugModeManager.Disable();
		}
		else
		{
			_debugModeManager.Enable();
		}
	}
}
