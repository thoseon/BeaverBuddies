using System.Collections.Generic;
using Timberborn.KeyBindingSystem;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Timberborn.InputSystem;

public class InputService : IUpdatableSingleton
{
	private static readonly string MouseLeftKey = "MouseLeft";

	private static readonly string MouseMiddleKey = "MouseMiddle";

	private static readonly string MouseRightKey = "MouseRight";

	private static readonly string ConfirmKey = "Confirm";

	private static readonly string CancelKey = "Cancel";

	private readonly InputSettings _inputSettings;

	private readonly InputUpdater _inputUpdater;

	private readonly MouseController _mouse;

	private readonly KeyBindingRegistry _keyBindingRegistry;

	private readonly InputBlocker _inputBlocker;

	private readonly List<IPriorityInputProcessor> _priorityInputProcessors = new List<IPriorityInputProcessor>();

	private readonly List<IInputProcessor> _inputProcessors = new List<IInputProcessor>();

	private readonly List<IInputProcessor> _inputProcessorsCopy = new List<IInputProcessor>();

	public bool MouseOverUI { get; private set; }

	public string MouseRotateCameraKey
	{
		get
		{
			if (!_inputSettings.SwapMouseCameraMovementWithRotation)
			{
				return MouseRightKey;
			}
			return MouseMiddleKey;
		}
	}

	public string MouseMoveCameraKey
	{
		get
		{
			if (!_inputSettings.SwapMouseCameraMovementWithRotation)
			{
				return MouseMiddleKey;
			}
			return MouseRightKey;
		}
	}

	public bool UIConfirm => IsKeyDown(ConfirmKey);

	public bool UICancel => KeyboardCancel;

	public bool Cancel
	{
		get
		{
			if (!MouseCancel)
			{
				return KeyboardCancel;
			}
			return true;
		}
	}

	public Vector3 MousePosition => _mouse.Position;

	public bool MainMouseButtonDown => IsKeyDown(MouseLeftKey);

	public bool MainMouseButtonHeld => IsKeyHeld(MouseLeftKey);

	public bool MainMouseButtonUp => IsKeyUp(MouseLeftKey);

	public bool MoveButtonHeld => IsKeyHeld(MouseMoveCameraKey);

	public bool RotateButtonDown => IsKeyDown(MouseRotateCameraKey);

	public bool RotateButtonHeld => IsKeyHeld(MouseRotateCameraKey);

	public bool RotateButtonLongHeld => IsKeyLongHeld(MouseRotateCameraKey);

	public bool RotateButtonUp => IsKeyUp(MouseRotateCameraKey);

	public bool WasConfirmPressedLastFrame
	{
		get
		{
			if (!IsKeyHeld(ConfirmKey))
			{
				return IsKeyUp(ConfirmKey);
			}
			return true;
		}
	}

	public bool WasCancelPressedLastFrame
	{
		get
		{
			if (!IsKeyHeld(CancelKey))
			{
				return IsKeyUp(CancelKey);
			}
			return true;
		}
	}

	public bool ScrollWheelActive => MouseZoom != 0f;

	public Vector2 MouseXYAxes => _mouse.XYAxes;

	public float MouseZoom
	{
		get
		{
			float num = (_inputSettings.InvertZoom ? (0f - _inputSettings.MouseWheelCameraZoomSpeed) : _inputSettings.MouseWheelCameraZoomSpeed);
			float num2 = ProcessScrollWheelAxis(_keyBindingRegistry.GetRawValue("MouseZoomIn"));
			if (num2 > 0f)
			{
				return num2 * num;
			}
			return ProcessScrollWheelAxis(_keyBindingRegistry.GetRawValue("MouseZoomOut")) * -1f * num;
		}
	}

	public Vector2 MousePositionNdc
	{
		get
		{
			Vector3 position = _mouse.Position;
			return new Vector2(position.x / (float)Screen.width, position.y / (float)Screen.height);
		}
	}

	private bool MouseCancel => IsKeyUpAfterShortHeld(MouseRightKey);

	private bool KeyboardCancel => IsKeyDown(CancelKey);

	public InputService(InputSettings inputSettings, InputUpdater inputUpdater, MouseController mouse, KeyBindingRegistry keyBindingRegistry, InputBlocker inputBlocker)
	{
		_inputSettings = inputSettings;
		_inputUpdater = inputUpdater;
		_mouse = mouse;
		_keyBindingRegistry = keyBindingRegistry;
		_inputBlocker = inputBlocker;
	}

	public bool IsKeyDown(string keyId)
	{
		return _keyBindingRegistry.IsDown(keyId);
	}

	public bool IsKeyUp(string keyId)
	{
		return _keyBindingRegistry.IsUp(keyId);
	}

	public bool IsKeyHeld(string keyId)
	{
		return _keyBindingRegistry.IsHeld(keyId);
	}

	public void UpdateSingleton()
	{
		MouseOverUI = EventSystem.current.IsPointerOverGameObject();
		if (!_inputBlocker.IsBlocked)
		{
			CallInputProcessors();
		}
		_inputUpdater.Update();
	}

	public void HideCursor()
	{
		_mouse.HideCursor();
	}

	public void ShowCursor()
	{
		_mouse.ShowCursor();
	}

	public void LockCursor()
	{
		_mouse.LockCursor();
	}

	public void UnlockCursor()
	{
		_mouse.UnlockCursor();
	}

	public void FlushUIInput()
	{
		_keyBindingRegistry.Get(ConfirmKey).Flush();
		_keyBindingRegistry.Get(CancelKey).Flush();
	}

	public void AddInputProcessor(IInputProcessor inputProcessor)
	{
		_inputProcessors.Add(inputProcessor);
	}

	public void AddInputProcessor(IPriorityInputProcessor inputProcessor)
	{
		_priorityInputProcessors.Add(inputProcessor);
	}

	public void RemoveInputProcessor(IInputProcessor inputProcessor)
	{
		for (int num = _inputProcessors.Count - 1; num >= 0; num--)
		{
			if (_inputProcessors[num] == inputProcessor)
			{
				_inputProcessors.RemoveAt(num);
				break;
			}
		}
	}

	private bool IsKeyLongHeld(string keyId)
	{
		return _keyBindingRegistry.IsLongHeld(keyId);
	}

	private bool IsKeyUpAfterShortHeld(string keyId)
	{
		return _keyBindingRegistry.IsUpAfterShortHeld(keyId);
	}

	private static float ProcessScrollWheelAxis(float rawAxisValue)
	{
		if (rawAxisValue < 0.001f)
		{
			return 0f;
		}
		return rawAxisValue / ScrollWheelSpeed.NormalizedScrollAxis.Value;
	}

	private void CallInputProcessors()
	{
		_inputProcessorsCopy.AddRange(_inputProcessors);
		try
		{
			for (int i = 0; i < _priorityInputProcessors.Count; i++)
			{
				_priorityInputProcessors[i].ProcessInput();
			}
			int num = _inputProcessorsCopy.Count - 1;
			while (num >= 0 && !_inputProcessorsCopy[num].ProcessInput())
			{
				num--;
			}
		}
		finally
		{
			_inputProcessorsCopy.Clear();
		}
	}
}
