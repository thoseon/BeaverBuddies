using Timberborn.SingletonSystem;
using UnityEngine.InputSystem;

namespace Timberborn.KeyBindingSystem;

internal class KeyBindingDeviceUpdater : ILoadableSingleton, IUnloadableSingleton
{
	private readonly KeyBindingRegistry _keyBindingRegistry;

	public KeyBindingDeviceUpdater(KeyBindingRegistry keyBindingRegistry)
	{
		_keyBindingRegistry = keyBindingRegistry;
	}

	public void Load()
	{
		InputSystem.onDeviceChange += OnDeviceChange;
	}

	public void Unload()
	{
		InputSystem.onDeviceChange -= OnDeviceChange;
	}

	private void OnDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
	{
		switch (inputDeviceChange)
		{
		case InputDeviceChange.Added:
			NotifyDeviceAdded();
			break;
		case InputDeviceChange.Removed:
			NotifyDeviceRemoved(inputDevice);
			break;
		}
	}

	private void NotifyDeviceAdded()
	{
		foreach (KeyBinding keyBinding in _keyBindingRegistry.KeyBindings)
		{
			keyBinding.PrimaryInputBinding.DeviceAdded();
			keyBinding.SecondaryInputBinding.DeviceAdded();
		}
	}

	private void NotifyDeviceRemoved(InputDevice inputDevice)
	{
		foreach (KeyBinding keyBinding in _keyBindingRegistry.KeyBindings)
		{
			keyBinding.PrimaryInputBinding.DeviceRemoved(inputDevice);
			keyBinding.SecondaryInputBinding.DeviceRemoved(inputDevice);
		}
	}
}
