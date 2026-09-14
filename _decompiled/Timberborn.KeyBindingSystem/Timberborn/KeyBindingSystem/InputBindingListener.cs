using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace Timberborn.KeyBindingSystem;

public class InputBindingListener : ILoadableSingleton, IUnloadableSingleton
{
	private static readonly string CancelKey = "Cancel";

	private static readonly string ConfirmKey = "Confirm";

	private readonly InputBindingNameService _inputBindingNameService;

	private readonly InputModifiersService _inputModifiersService;

	private readonly KeyBindingRegistry _keyBindingRegistry;

	private readonly HashSet<Key> _pressedModifiers = new HashSet<Key>();

	private Action<CustomInputBinding> _callback;

	private KeyBinding _cancelKeyBinding;

	private KeyBinding _confirmKeyBinding;

	public InputBindingListener(InputBindingNameService inputBindingNameService, InputModifiersService inputModifiersService, KeyBindingRegistry keyBindingRegistry)
	{
		_inputBindingNameService = inputBindingNameService;
		_inputModifiersService = inputModifiersService;
		_keyBindingRegistry = keyBindingRegistry;
	}

	public void Load()
	{
		_cancelKeyBinding = _keyBindingRegistry.Get(CancelKey);
		_confirmKeyBinding = _keyBindingRegistry.Get(ConfirmKey);
	}

	public void WaitForInput(Action<CustomInputBinding> callback)
	{
		Asserts.FieldIsNull(this, _callback, "_callback");
		_callback = callback;
		InputSystem.onEvent += new Action<InputEventPtr, InputDevice>(OnInputSystemEvent);
	}

	public void FinishListening()
	{
		Asserts.FieldIsNotNull(this, _callback, "_callback");
		_callback = null;
		_pressedModifiers.Clear();
		InputSystem.onEvent -= new Action<InputEventPtr, InputDevice>(OnInputSystemEvent);
	}

	public void Unload()
	{
		InputSystem.onEvent -= new Action<InputEventPtr, InputDevice>(OnInputSystemEvent);
	}

	private void OnInputSystemEvent(InputEventPtr inputEvent, InputDevice device)
	{
		if (!inputEvent.IsAnyStateEvent())
		{
			return;
		}
		foreach (InputControl item in inputEvent.EnumerateChangedControls())
		{
			if (ValidateInput(inputEvent, item))
			{
				break;
			}
		}
	}

	private bool ValidateInput(InputEventPtr inputEvent, InputControl inputControl)
	{
		bool flag = inputControl.WasPressedInEvent(inputEvent);
		if (_inputModifiersService.IsModifier(inputControl))
		{
			return ValidateModifierKey(inputControl as KeyControl, flag);
		}
		if (flag && IsValidInput(inputControl))
		{
			NotifyAndFinishListening(GetInputToNotify(inputEvent, inputControl), _inputModifiersService.PressedModifiers());
			return true;
		}
		return false;
	}

	private bool ValidateModifierKey(KeyControl keyControl, bool wasPressed)
	{
		if (wasPressed)
		{
			_pressedModifiers.Add(keyControl.keyCode);
			return false;
		}
		_pressedModifiers.Remove(keyControl.keyCode);
		if (_pressedModifiers.Count == 0)
		{
			NotifyAndFinishListening(keyControl, InputModifiers.None);
			return true;
		}
		return false;
	}

	private bool IsValidInput(InputControl inputControl)
	{
		if (!IsButton(inputControl) || !IsNotCancelConfirmButton(inputControl) || !IsNotMainMouseButton(inputControl))
		{
			return IsMouseScroll(inputControl);
		}
		return true;
	}

	private static InputControl GetInputToNotify(InputEventPtr inputEvent, InputControl inputControl)
	{
		if (!IsMouseScroll(inputControl))
		{
			return inputControl;
		}
		return ConvertMouseScroll(inputEvent, inputControl);
	}

	private void NotifyAndFinishListening(InputControl inputControl, InputModifiers inputModifiers)
	{
		string inputControlName = _inputBindingNameService.GetInputControlName(inputControl);
		_callback(new CustomInputBinding(inputControl.path, inputModifiers, inputControlName));
		FinishListening();
	}

	private static bool IsButton(InputControl inputControl)
	{
		if (inputControl is ButtonControl)
		{
			return !inputControl.synthetic;
		}
		return false;
	}

	private bool IsNotCancelConfirmButton(InputControl inputControl)
	{
		if (_cancelKeyBinding.PrimaryInputBinding.InputControl != inputControl && _cancelKeyBinding.SecondaryInputBinding.InputControl != inputControl && _confirmKeyBinding.PrimaryInputBinding.InputControl != inputControl)
		{
			return _confirmKeyBinding.SecondaryInputBinding.InputControl != inputControl;
		}
		return false;
	}

	private static bool IsNotMainMouseButton(InputControl inputControl)
	{
		if (inputControl.device is Mouse mouse)
		{
			if (mouse.leftButton != inputControl)
			{
				return mouse.rightButton != inputControl;
			}
			return false;
		}
		return true;
	}

	private static bool IsMouseScroll(InputControl inputControl)
	{
		if (inputControl.device is Mouse mouse)
		{
			return mouse.scroll.children.Contains(inputControl);
		}
		return false;
	}

	private static InputControl ConvertMouseScroll(InputEventPtr inputEvent, InputControl inputControl)
	{
		if (inputControl.device is Mouse mouse)
		{
			if (inputControl == mouse.scroll.y)
			{
				float num = mouse.scroll.y.ReadValueFromEvent(inputEvent);
				if (num > 0f)
				{
					return mouse.scroll.up;
				}
				if (num < 0f)
				{
					return mouse.scroll.down;
				}
			}
			if (inputControl == mouse.scroll.x)
			{
				float num2 = mouse.scroll.x.ReadValueFromEvent(inputEvent);
				if (num2 > 0f)
				{
					return mouse.scroll.right;
				}
				if (num2 < 0f)
				{
					return mouse.scroll.left;
				}
			}
		}
		throw new InvalidOperationException("Failed to convert mouse scroll to input control");
	}
}
