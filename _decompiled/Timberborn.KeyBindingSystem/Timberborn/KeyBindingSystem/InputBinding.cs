using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Timberborn.KeyBindingSystem;

public class InputBinding
{
	private readonly bool _allowOtherModifiers;

	private readonly float _pressedButtonThreshold = 0.05f;

	public InputControl InputControl { get; private set; }

	public InputBindingSpec InputBindingSpec { get; }

	public string DefaultName { get; }

	public bool IsDefined => InputBindingSpec.IsDefined;

	private InputModifiers InputModifiers => InputBindingSpec.InputModifiers;

	private InputBinding(InputBindingSpec inputBindingSpec, string defaultName, bool allowOtherModifiers)
	{
		InputBindingSpec = inputBindingSpec;
		DefaultName = defaultName;
		_allowOtherModifiers = allowOtherModifiers;
	}

	public static InputBinding Create(InputBindingSpec inputBindingSpec, string defaultName, bool allowOtherModifiers)
	{
		InputBinding inputBinding = new InputBinding(inputBindingSpec ?? new InputBindingSpec(), defaultName, allowOtherModifiers);
		inputBinding.SearchForInputControl();
		return inputBinding;
	}

	public bool IsPressed(InputModifiers currentModifiers)
	{
		if (CanBePressed(currentModifiers))
		{
			return InputControl.IsPressed(_pressedButtonThreshold);
		}
		return false;
	}

	public bool WasPressedInEvent(InputEventPtr inputEvent, InputModifiers currentModifiers)
	{
		if (CanBePressed(currentModifiers))
		{
			return InputControl.WasPressedInEvent(inputEvent);
		}
		return false;
	}

	public void DeviceRemoved(InputDevice device)
	{
		if (InputControl?.device == device)
		{
			InputControl = null;
		}
		SearchForInputControl();
	}

	public void DeviceAdded()
	{
		if (InputControl == null)
		{
			SearchForInputControl();
		}
	}

	public bool HasModifier(InputModifiers modifier)
	{
		return (InputModifiers & modifier) == modifier;
	}

	public float GetRawValue()
	{
		if (InputControl is InputControl<float> inputControl)
		{
			return inputControl.value;
		}
		return 0f;
	}

	private void SearchForInputControl()
	{
		InputControl = (IsDefined ? InputSystem.FindControl(InputBindingSpec.Path) : null);
	}

	private bool CanBePressed(InputModifiers currentModifiers)
	{
		if (InputControl != null)
		{
			return AreModifiersMatching(currentModifiers);
		}
		return false;
	}

	private bool AreModifiersMatching(InputModifiers currentModifiers)
	{
		if (_allowOtherModifiers)
		{
			return (currentModifiers & InputModifiers) == InputModifiers;
		}
		return currentModifiers == InputModifiers;
	}
}
