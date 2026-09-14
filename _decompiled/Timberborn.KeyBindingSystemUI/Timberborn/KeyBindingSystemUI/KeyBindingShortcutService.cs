using Timberborn.KeyBindingSystem;
using UnityEngine.UIElements;

namespace Timberborn.KeyBindingSystemUI;

public class KeyBindingShortcutService
{
	private readonly InputBindingDescriber _inputBindingDescriber;

	private readonly KeyBindingRegistry _keyBindingRegistry;

	private readonly KeyBindingShortcutUpdater _keyBindingShortcutUpdater;

	public KeyBindingShortcutService(InputBindingDescriber inputBindingDescriber, KeyBindingRegistry keyBindingRegistry, KeyBindingShortcutUpdater keyBindingShortcutUpdater)
	{
		_inputBindingDescriber = inputBindingDescriber;
		_keyBindingRegistry = keyBindingRegistry;
		_keyBindingShortcutUpdater = keyBindingShortcutUpdater;
	}

	public void CreateSingle(TextElement textElement, DefinableInputBinding definableInputBinding)
	{
		AddShortcut(new KeyBindingShortcut(_inputBindingDescriber, definableInputBinding, new ShortcutTextElement(textElement, alwaysVisible: true)));
	}

	public void CreateAny(TextElement textElement, string keyId)
	{
		AddShortcut(new KeyBindingShortcut(_inputBindingDescriber, new DefinableInputBinding(_keyBindingRegistry.Get(keyId), null), new ShortcutTextElement(textElement, alwaysVisible: false)));
	}

	private void AddShortcut(KeyBindingShortcut keyBindingShortcut)
	{
		_keyBindingShortcutUpdater.AddShortcut(keyBindingShortcut);
	}
}
