using Timberborn.KeyBindingSystem;

namespace Timberborn.KeyBindingSystemUI;

public class KeyBindingDescriber
{
	private readonly KeyBindingRegistry _keyBindingRegistry;

	private readonly InputBindingDescriber _inputBindingDescriber;

	public KeyBindingDescriber(KeyBindingRegistry keyBindingRegistry, InputBindingDescriber inputBindingDescriber)
	{
		_keyBindingRegistry = keyBindingRegistry;
		_inputBindingDescriber = inputBindingDescriber;
	}

	public bool TryGetKeyBindingText(string keyBindingKey, out string keyBindingText)
	{
		if (keyBindingKey != null)
		{
			KeyBinding keyBinding = _keyBindingRegistry.Get(keyBindingKey);
			if (keyBinding.PrimaryInputBinding.IsDefined || keyBinding.SecondaryInputBinding.IsDefined)
			{
				InputBinding inputBinding = (keyBinding.PrimaryInputBinding.IsDefined ? keyBinding.PrimaryInputBinding : keyBinding.SecondaryInputBinding);
				keyBindingText = _inputBindingDescriber.GetInputBindingText(inputBinding);
				return true;
			}
		}
		keyBindingText = null;
		return false;
	}
}
