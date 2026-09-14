using System;
using Timberborn.SingletonSystem;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Timberborn.KeyBindingSystem;

public class InputUpdater : ILoadableSingleton, IUnloadableSingleton
{
	private readonly InputModifiersService _inputModifiersService;

	private readonly KeyBindingRegistry _keyBindingRegistry;

	public InputUpdater(InputModifiersService inputModifiersService, KeyBindingRegistry keyBindingRegistry)
	{
		_inputModifiersService = inputModifiersService;
		_keyBindingRegistry = keyBindingRegistry;
	}

	public void Update()
	{
		InputModifiers inputModifiers = _inputModifiersService.PressedModifiers();
		foreach (KeyBinding keyBinding in _keyBindingRegistry.KeyBindings)
		{
			keyBinding.UpdateKeyState(inputModifiers);
		}
	}

	public void Load()
	{
		InputSystem.onEvent += new Action<InputEventPtr, InputDevice>(OnInputSystemEvent);
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
		InputModifiers inputModifiers = _inputModifiersService.PressedModifiers();
		foreach (InputControl item in inputEvent.EnumerateChangedControls())
		{
			UpdateEventStates(inputEvent, item, inputModifiers);
		}
	}

	private void UpdateEventStates(InputEventPtr inputEvent, InputControl changedControl, InputModifiers inputModifiers)
	{
		foreach (KeyBinding keyBinding in _keyBindingRegistry.KeyBindings)
		{
			keyBinding.UpdateEventState(inputEvent, changedControl, inputModifiers);
		}
	}
}
