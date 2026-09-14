using System;
using Timberborn.Localization;
using Timberborn.PlatformUtilities;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Timberborn.KeyBindingSystem;

public class InputBindingNameService
{
	private static readonly string MouseScrollDownLocKey = "Mouse.ScrollDown";

	private static readonly string MouseScrollLeftLocKey = "Mouse.ScrollLeft";

	private static readonly string MouseScrollRightLocKey = "Mouse.ScrollRight";

	private static readonly string MouseScrollUpLocKey = "Mouse.ScrollUp";

	private static readonly string KeySpaceLocKey = "Key.Space";

	private static readonly string KeyLeftArrowLocKey = "Key.LeftArrow";

	private static readonly string KeyRightArrowLocKey = "Key.RightArrow";

	private static readonly string KeyUpArrowLocKey = "Key.UpArrow";

	private static readonly string KeyDownArrowLocKey = "Key.DownArrow";

	private readonly ILoc _loc;

	public InputBindingNameService(ILoc loc)
	{
		_loc = loc;
	}

	public string GetName(InputBinding inputBinding)
	{
		if (!string.IsNullOrEmpty(inputBinding.DefaultName))
		{
			return inputBinding.DefaultName;
		}
		if (inputBinding.InputControl != null)
		{
			return GetInputControlName(inputBinding.InputControl);
		}
		return inputBinding.InputBindingSpec.Path;
	}

	public string GetInputControlName(InputControl inputControl)
	{
		string buttonName = GetButtonName(inputControl);
		string devicePrefix = GetDevicePrefix(inputControl);
		if (!string.IsNullOrEmpty(devicePrefix))
		{
			return devicePrefix + ": " + buttonName;
		}
		return buttonName;
	}

	public string GetInputModifierName(InputModifiers inputModifier)
	{
		bool flag = ApplicationPlatform.IsMacOS();
		return inputModifier switch
		{
			InputModifiers.Ctrl => "Ctrl", 
			InputModifiers.Alt => flag ? "Option" : "Alt", 
			InputModifiers.Shift => "Shift", 
			InputModifiers.Cmd => "Cmd", 
			InputModifiers.None => throw new NotSupportedException(), 
			_ => throw new ArgumentOutOfRangeException("inputModifier", inputModifier, null), 
		};
	}

	private string GetButtonName(InputControl inputControl)
	{
		if (TryGetButtonLocKey(inputControl, out var locKey))
		{
			return _loc.T(locKey);
		}
		return GetKeyCodeName(inputControl) ?? inputControl.shortDisplayName ?? inputControl.displayName;
	}

	private static bool TryGetButtonLocKey(InputControl inputControl, out string locKey)
	{
		if (!TryGetKeyLocKey(inputControl, out locKey))
		{
			return TryGetMouseLocKey(inputControl, out locKey);
		}
		return true;
	}

	private static bool TryGetKeyLocKey(InputControl inputControl, out string locKey)
	{
		if (inputControl is KeyControl keyControl)
		{
			locKey = GetKeyCodeLocKey(keyControl.keyCode);
			return locKey != null;
		}
		locKey = null;
		return false;
	}

	private static bool TryGetMouseLocKey(InputControl inputControl, out string locKey)
	{
		if (inputControl.device is Mouse mouse)
		{
			if (inputControl == mouse.scroll.down)
			{
				locKey = MouseScrollDownLocKey;
				return true;
			}
			if (inputControl == mouse.scroll.left)
			{
				locKey = MouseScrollLeftLocKey;
				return true;
			}
			if (inputControl == mouse.scroll.right)
			{
				locKey = MouseScrollRightLocKey;
				return true;
			}
			if (inputControl == mouse.scroll.up)
			{
				locKey = MouseScrollUpLocKey;
				return true;
			}
		}
		locKey = null;
		return false;
	}

	private static string GetKeyCodeLocKey(Key keyCode)
	{
		return keyCode switch
		{
			Key.Space => KeySpaceLocKey, 
			Key.LeftArrow => KeyLeftArrowLocKey, 
			Key.RightArrow => KeyRightArrowLocKey, 
			Key.UpArrow => KeyUpArrowLocKey, 
			Key.DownArrow => KeyDownArrowLocKey, 
			_ => null, 
		};
	}

	private string GetKeyCodeName(InputControl inputControl)
	{
		if (inputControl is KeyControl keyControl)
		{
			bool flag = ApplicationPlatform.IsMacOS();
			return keyControl.keyCode switch
			{
				Key.LeftAlt => GetLeftInputModifierName(InputModifiers.Alt), 
				Key.RightAlt => GetRightInputModifierName(InputModifiers.Alt), 
				Key.LeftCtrl => GetLeftInputModifierName(InputModifiers.Ctrl), 
				Key.RightCtrl => GetRightInputModifierName(InputModifiers.Ctrl), 
				Key.LeftShift => GetLeftInputModifierName(InputModifiers.Shift), 
				Key.RightShift => GetRightInputModifierName(InputModifiers.Shift), 
				Key.LeftMeta => flag ? GetLeftInputModifierName(InputModifiers.Cmd) : _loc.T("Key.LeftWindows"), 
				Key.RightMeta => flag ? GetRightInputModifierName(InputModifiers.Cmd) : _loc.T("Key.RightWindows"), 
				_ => null, 
			};
		}
		return null;
	}

	private string GetLeftInputModifierName(InputModifiers inputModifier)
	{
		return "L-" + GetInputModifierName(inputModifier);
	}

	private string GetRightInputModifierName(InputModifiers inputModifier)
	{
		return "R-" + GetInputModifierName(inputModifier);
	}

	private static string GetDevicePrefix(InputControl inputControl)
	{
		InputDevice device = inputControl.device;
		if ((!(device is Keyboard) && !(device is Mouse)) || 1 == 0)
		{
			return inputControl.device.displayName;
		}
		return string.Empty;
	}
}
