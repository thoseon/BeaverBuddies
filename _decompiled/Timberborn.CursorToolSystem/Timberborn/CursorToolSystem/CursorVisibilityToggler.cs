using Timberborn.InputSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.CursorToolSystem;

public class CursorVisibilityToggler : IInputProcessor, ILoadableSingleton
{
	private static readonly string ToggleCursorVisibilityKey = "ToggleCursorVisibility";

	private readonly InputService _inputService;

	private readonly MouseController _mouseController;

	public CursorVisibilityToggler(InputService inputService, MouseController mouseController)
	{
		_inputService = inputService;
		_mouseController = mouseController;
	}

	public void Load()
	{
		_inputService.AddInputProcessor(this);
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(ToggleCursorVisibilityKey))
		{
			ToggleCursorVisibility();
			return true;
		}
		return false;
	}

	private void ToggleCursorVisibility()
	{
		if (_mouseController.IsCursorVisible)
		{
			_mouseController.ForceHideCursor();
		}
		else
		{
			_mouseController.ForceShowCursor();
		}
	}
}
