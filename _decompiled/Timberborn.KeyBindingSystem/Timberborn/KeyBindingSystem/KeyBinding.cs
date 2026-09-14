using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Timberborn.KeyBindingSystem;

public class KeyBinding
{
	private static readonly float LongHeldThreshold = 0.2f;

	private readonly KeyBindingDefinition _keyBindingDefinition;

	private readonly bool _isHidden;

	private float _holdingStartTime;

	private bool _isLocked;

	private InputModifiers _inputModifiers;

	public bool IsDown { get; private set; }

	public bool IsHeld { get; private set; }

	public bool IsLongHeld { get; private set; }

	public bool IsUp { get; private set; }

	public bool IsUpAfterShortHeld { get; private set; }

	public string DisplayName { get; private set; }

	public InputBinding PrimaryInputBinding => _keyBindingDefinition.PrimaryInputBinding;

	public InputBinding SecondaryInputBinding => _keyBindingDefinition.SecondaryInputBinding;

	public string Id => _keyBindingDefinition.KeyBindingSpec.Id;

	public string GroupId => _keyBindingDefinition.KeyBindingSpec.GroupId;

	public bool DevModeOnly => _keyBindingDefinition.KeyBindingSpec.DevModeOnly;

	public KeyBinding(string displayName, KeyBindingDefinition keyBindingDefinition, bool isHidden)
	{
		DisplayName = displayName;
		_keyBindingDefinition = keyBindingDefinition;
		_isHidden = isHidden;
	}

	public void Lock()
	{
		UpdateUnpressedState();
		_isLocked = true;
	}

	public float GetRawValue()
	{
		if (Application.isFocused && _inputModifiers == InputModifiers.None)
		{
			return Mathf.Max(PrimaryInputBinding.GetRawValue(), SecondaryInputBinding.GetRawValue());
		}
		return 0f;
	}

	public void UpdateKeyState(InputModifiers inputModifiers)
	{
		_inputModifiers = inputModifiers;
		if (Application.isFocused && IsPressed(inputModifiers))
		{
			UpdatePressedState();
		}
		else
		{
			UpdateUnpressedState();
		}
	}

	public void UpdateEventState(InputEventPtr inputEvent, InputControl changedControl, InputModifiers inputModifiers)
	{
		_inputModifiers = inputModifiers;
		if (Application.isFocused && IsUsingInput(changedControl) && WasPressedInEvent(inputEvent, inputModifiers))
		{
			UpdatePressedState();
		}
	}

	public bool IsUsingBinding(CustomInputBinding customInputBinding)
	{
		if (!_isHidden)
		{
			if (!customInputBinding.IsSame(PrimaryInputBinding.InputBindingSpec))
			{
				return customInputBinding.IsSame(SecondaryInputBinding.InputBindingSpec);
			}
			return true;
		}
		return false;
	}

	public void Flush()
	{
		if (IsDown)
		{
			UpdatePressedState();
		}
	}

	private bool IsPressed(InputModifiers inputModifiers)
	{
		if (!PrimaryInputBinding.IsPressed(inputModifiers))
		{
			return SecondaryInputBinding.IsPressed(inputModifiers);
		}
		return true;
	}

	private bool IsUsingInput(InputControl inputControl)
	{
		if (PrimaryInputBinding.InputControl != inputControl)
		{
			return SecondaryInputBinding.InputControl == inputControl;
		}
		return true;
	}

	private bool WasPressedInEvent(InputEventPtr inputEvent, InputModifiers inputModifiers)
	{
		if (!PrimaryInputBinding.WasPressedInEvent(inputEvent, inputModifiers))
		{
			return SecondaryInputBinding.WasPressedInEvent(inputEvent, inputModifiers);
		}
		return true;
	}

	private void UpdatePressedState()
	{
		if (!_isLocked)
		{
			if (IsHeld)
			{
				IsDown = false;
			}
			else
			{
				IsHeld = true;
				IsDown = true;
				_holdingStartTime = Time.unscaledTime;
			}
			IsLongHeld = Time.unscaledTime - _holdingStartTime > LongHeldThreshold;
			IsUp = false;
			IsUpAfterShortHeld = false;
		}
	}

	private void UpdateUnpressedState()
	{
		IsUp = IsHeld;
		if (IsUp)
		{
			IsUpAfterShortHeld = !IsLongHeld;
		}
		else
		{
			IsUpAfterShortHeld = false;
		}
		IsDown = false;
		IsHeld = false;
		IsLongHeld = false;
		_isLocked = false;
	}
}
