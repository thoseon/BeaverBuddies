using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Timberborn.InputSystem;

internal class InputStateResetter : ILoadableSingleton, IUnloadableSingleton, IInputStateResetter
{
	public void Load()
	{
		Application.focusChanged += OnFocusChanged;
	}

	public void Unload()
	{
		Application.focusChanged -= OnFocusChanged;
	}

	public void ResetInputState()
	{
		foreach (InputDevice device in UnityEngine.InputSystem.InputSystem.devices)
		{
			UnityEngine.InputSystem.InputSystem.ResetDevice(device, alsoResetDontResetControls: true);
		}
	}

	private void OnFocusChanged(bool focus)
	{
		if (focus)
		{
			ResetInputState();
		}
	}
}
