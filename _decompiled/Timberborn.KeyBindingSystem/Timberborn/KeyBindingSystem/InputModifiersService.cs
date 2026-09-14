using Timberborn.PlatformUtilities;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Timberborn.KeyBindingSystem;

public class InputModifiersService
{
	public bool IsModifier(InputControl inputControl)
	{
		if (ApplicationPlatform.IsMacOS())
		{
			return (inputControl is KeyControl { keyCode: var keyCode } && (uint)(keyCode - 51) <= 7u) ? true : false;
		}
		return (inputControl is KeyControl { keyCode: var keyCode2 } && (uint)(keyCode2 - 51) <= 5u) ? true : false;
	}

	public InputModifiers PressedModifiers()
	{
		InputModifiers inputModifiers = InputModifiers.None;
		Keyboard current = Keyboard.current;
		if (current.ctrlKey.isPressed)
		{
			inputModifiers |= InputModifiers.Ctrl;
		}
		if (current.altKey.isPressed)
		{
			inputModifiers |= InputModifiers.Alt;
		}
		if (current.shiftKey.isPressed)
		{
			inputModifiers |= InputModifiers.Shift;
		}
		if (ApplicationPlatform.IsMacOS() && (current.leftCommandKey.isPressed || current.rightCommandKey.isPressed))
		{
			inputModifiers |= InputModifiers.Cmd;
		}
		return inputModifiers;
	}
}
