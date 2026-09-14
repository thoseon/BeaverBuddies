using Timberborn.InputSystem;
using Timberborn.PlatformUtilities;
using UnityEngine;

namespace Timberborn.CameraSystem;

public class CameraMovementInput
{
	private static readonly string RotateCameraRightKey = "RotateCameraRight";

	private static readonly string RotateCameraLeftKey = "RotateCameraLeft";

	private static readonly string RotateCameraUpKey = "RotateCameraUp";

	private static readonly string RotateCameraDownKey = "RotateCameraDown";

	private static readonly string RotateCameraRight90Key = "RotateCameraRight90";

	private static readonly string RotateCameraLeft90Key = "RotateCameraLeft90";

	private static readonly string MoveCameraFastKey = "MoveCameraFast";

	private static readonly string MoveCameraUpKey = "MoveCameraUp";

	private static readonly string MoveCameraDownKey = "MoveCameraDown";

	private static readonly string MoveCameraLeftKey = "MoveCameraLeft";

	private static readonly string MoveCameraRightKey = "MoveCameraRight";

	private readonly InputService _inputService;

	public bool MoveCameraFast => _inputService.IsKeyHeld(MoveCameraFastKey);

	public Vector2 CameraMovementAxes => new Vector2(GetHorizontalAxis(), GetVerticalAxis());

	private bool AxisKeyUp => _inputService.IsKeyHeld(MoveCameraUpKey);

	private bool AxisKeyDown => _inputService.IsKeyHeld(MoveCameraDownKey);

	private bool AxisKeyLeft => _inputService.IsKeyHeld(MoveCameraLeftKey);

	private bool AxisKeyRight => _inputService.IsKeyHeld(MoveCameraRightKey);

	public CameraMovementInput(InputService inputService)
	{
		_inputService = inputService;
	}

	public ScreenEdges GetMouseScreenEdges()
	{
		ScreenEdges screenEdges = ScreenEdges.None;
		if (Application.isFocused)
		{
			Vector3 mousePosition = _inputService.MousePosition;
			float x = mousePosition.x;
			float y = mousePosition.y;
			float num = (ApplicationPlatform.IsMacOS() ? 64f : 0f);
			if (y >= 0f - num && y <= 1f)
			{
				screenEdges |= ScreenEdges.Down;
			}
			if (x >= 0f - num && x <= 1f)
			{
				screenEdges |= ScreenEdges.Left;
			}
			int height = Screen.height;
			if (y >= (float)height - 1f && y <= (float)height + num)
			{
				screenEdges |= ScreenEdges.Up;
			}
			int width = Screen.width;
			if (x >= (float)width - 1f && x <= (float)width + num)
			{
				screenEdges |= ScreenEdges.Right;
			}
		}
		return screenEdges;
	}

	public Vector2 GetCameraRotationAxes()
	{
		if (_inputService.IsKeyHeld(RotateCameraRightKey))
		{
			return Vector2.right;
		}
		if (_inputService.IsKeyHeld(RotateCameraLeftKey))
		{
			return Vector2.left;
		}
		if (_inputService.IsKeyHeld(RotateCameraUpKey))
		{
			return Vector2.up;
		}
		if (_inputService.IsKeyHeld(RotateCameraDownKey))
		{
			return Vector2.down;
		}
		return Vector2.zero;
	}

	public Vector2 GetCameraJumpRotationAxes()
	{
		if (_inputService.IsKeyDown(RotateCameraRight90Key))
		{
			return Vector2.right;
		}
		if (_inputService.IsKeyDown(RotateCameraLeft90Key))
		{
			return Vector2.left;
		}
		return Vector2.zero;
	}

	private int GetHorizontalAxis()
	{
		if (AxisKeyRight && AxisKeyLeft)
		{
			return 0;
		}
		if (AxisKeyRight)
		{
			return 1;
		}
		if (AxisKeyLeft)
		{
			return -1;
		}
		return 0;
	}

	private int GetVerticalAxis()
	{
		if (AxisKeyUp && AxisKeyDown)
		{
			return 0;
		}
		if (AxisKeyUp)
		{
			return 1;
		}
		if (AxisKeyDown)
		{
			return -1;
		}
		return 0;
	}
}
