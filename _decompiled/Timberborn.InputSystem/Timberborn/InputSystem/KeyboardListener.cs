using System;
using Timberborn.KeyBindingSystem;
using Timberborn.SingletonSystem;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace Timberborn.InputSystem;

public class KeyboardListener : ILoadableSingleton, IUnloadableSingleton
{
	public event EventHandler<KeyPressedEvent> KeyPressed;

	public void Load()
	{
		UnityEngine.InputSystem.InputSystem.onEvent += new Action<InputEventPtr, InputDevice>(OnInputSystemEvent);
	}

	public void Unload()
	{
		UnityEngine.InputSystem.InputSystem.onEvent -= new Action<InputEventPtr, InputDevice>(OnInputSystemEvent);
	}

	private void OnInputSystemEvent(InputEventPtr inputEvent, InputDevice inputDevice)
	{
		if (inputDevice == Keyboard.current && inputEvent.IsAnyStateEvent())
		{
			CollectKeys(inputEvent);
		}
	}

	private void CollectKeys(InputEventPtr inputEvent)
	{
		ReadOnlyArray<KeyControl> allKeys = Keyboard.current.allKeys;
		for (int i = 0; i < allKeys.Count; i++)
		{
			KeyControl keyControl = allKeys[i];
			if (keyControl != null)
			{
				bool isPressed = keyControl.isPressed;
				bool flag = keyControl.IsValueConsideredPressed(keyControl.ReadValueFromEvent(inputEvent));
				if (!isPressed & flag)
				{
					KeyPressed?.Invoke(this, new KeyPressedEvent(keyControl.displayName.ToUpper()));
				}
			}
		}
	}
}
