using System;
using System.Collections.Immutable;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.InputSystem;

public class InputSettings : ILoadableSingleton
{
	public static readonly ImmutableArray<string> OnScreenKeyboardValues = ImmutableArray.Create("Auto", "Enabled", "Disabled");

	private static readonly string OnScreenKeyboardDefaultValue = "Auto";

	private static readonly string InvertZoomKey = "InvertZoom";

	private static readonly string SwapMouseCameraMovementWithRotationKey = "SwapMouseCameraMovementWithRotation";

	private static readonly string DragCameraKey = "DragCamera";

	private static readonly string LockCursorInWindowKey = "LockCursorInWindow";

	private static readonly string EdgePanCameraKey = "EdgePanCamera";

	private static readonly string OnScreenKeyboardKey = "OnScreenKeyboard";

	private static readonly string MouseCameraRotationSpeedKey = "MouseCameraRotationSpeed";

	private static readonly string EdgePanCameraSpeedKey = "EdgePanCameraSpeed";

	private static readonly string KeyboardCameraMovementSpeedKey = "KeyboardCameraMovementSpeed";

	private static readonly string KeyboardCameraRotationSpeedKey = "KeyboardCameraRotationSpeed";

	private static readonly string KeyboardCameraZoomSpeedKey = "KeyboardCameraZoomSpeed";

	private static readonly string MouseWheelCameraZoomSpeedKey = "MouseWheelCameraZoomSpeed";

	private readonly ISettings _settings;

	public bool InvertZoom
	{
		get
		{
			return _settings.GetBool(InvertZoomKey);
		}
		set
		{
			_settings.SetBool(InvertZoomKey, value);
		}
	}

	public bool SwapMouseCameraMovementWithRotation
	{
		get
		{
			return _settings.GetBool(SwapMouseCameraMovementWithRotationKey);
		}
		set
		{
			_settings.SetBool(SwapMouseCameraMovementWithRotationKey, value);
		}
	}

	public bool DragCamera
	{
		get
		{
			return _settings.GetBool(DragCameraKey);
		}
		set
		{
			_settings.SetBool(DragCameraKey, value);
		}
	}

	public bool LockCursorInWindow
	{
		get
		{
			return _settings.GetBool(LockCursorInWindowKey);
		}
		set
		{
			_settings.SetBool(LockCursorInWindowKey, value);
			LockCursorInWindowChanged?.Invoke(this, new SettingChangedEventArgs<bool>(value));
		}
	}

	public bool EdgePanCamera
	{
		get
		{
			return _settings.GetBool(EdgePanCameraKey);
		}
		set
		{
			_settings.SetBool(EdgePanCameraKey, value);
		}
	}

	public float EdgePanCameraSpeed
	{
		get
		{
			return _settings.GetFloat(EdgePanCameraSpeedKey, 0.4f);
		}
		set
		{
			_settings.SetFloat(EdgePanCameraSpeedKey, value);
		}
	}

	public float KeyboardCameraMovementSpeed
	{
		get
		{
			return _settings.GetFloat(KeyboardCameraMovementSpeedKey, 0.4f);
		}
		set
		{
			_settings.SetFloat(KeyboardCameraMovementSpeedKey, value);
		}
	}

	public float KeyboardCameraRotationSpeed
	{
		get
		{
			return _settings.GetFloat(KeyboardCameraRotationSpeedKey, 0.4f);
		}
		set
		{
			_settings.SetFloat(KeyboardCameraRotationSpeedKey, value);
		}
	}

	public float KeyboardCameraZoomSpeed
	{
		get
		{
			return _settings.GetFloat(KeyboardCameraZoomSpeedKey, 0.4f);
		}
		set
		{
			_settings.SetFloat(KeyboardCameraZoomSpeedKey, value);
		}
	}

	public float MouseWheelCameraZoomSpeed
	{
		get
		{
			return _settings.GetFloat(MouseWheelCameraZoomSpeedKey, 0.4f);
		}
		set
		{
			_settings.SetFloat(MouseWheelCameraZoomSpeedKey, value);
		}
	}

	public float MouseCameraRotationSpeed
	{
		get
		{
			return _settings.GetFloat(MouseCameraRotationSpeedKey, 0.4f);
		}
		set
		{
			_settings.SetFloat(MouseCameraRotationSpeedKey, value);
		}
	}

	public string OnScreenKeyboard
	{
		get
		{
			return _settings.GetSafeString(OnScreenKeyboardKey, OnScreenKeyboardDefaultValue);
		}
		set
		{
			_settings.SetString(OnScreenKeyboardKey, value);
		}
	}

	public event EventHandler<SettingChangedEventArgs<bool>> LockCursorInWindowChanged;

	public InputSettings(ISettings settings)
	{
		_settings = settings;
	}

	public void Load()
	{
		_settings.ValidateString(OnScreenKeyboardKey, OnScreenKeyboardValues, OnScreenKeyboardDefaultValue);
	}
}
