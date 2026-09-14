using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace Timberborn.KeyBindingSystem;

internal static class InputControlExtensions
{
	public static bool WasPressedInEvent(this InputControl inputControl, InputEventPtr inputEvent)
	{
		if (inputControl is InputControl<float> control)
		{
			float num = control.ReadValueFromEvent(inputEvent);
			if (inputControl is ButtonControl buttonControl)
			{
				return buttonControl.IsValueConsideredPressed(num);
			}
			return num != 0f;
		}
		return false;
	}
}
