using System.Collections.Generic;
using Timberborn.KeyBindingSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.KeyBindingSystemUI;

public class KeyBindingShortcutUpdater : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly List<KeyBindingShortcut> _keyBindingShortcuts = new List<KeyBindingShortcut>();

	public KeyBindingShortcutUpdater(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void AddShortcut(KeyBindingShortcut keyBindingShortcut)
	{
		keyBindingShortcut.Update();
		_keyBindingShortcuts.Add(keyBindingShortcut);
	}

	[OnEvent]
	public void OnKeyRebound(KeyReboundEvent keyReboundEvent)
	{
		foreach (KeyBindingShortcut keyBindingShortcut in _keyBindingShortcuts)
		{
			if (keyBindingShortcut.KeyBindingId == keyReboundEvent.KeyBindingId)
			{
				keyBindingShortcut.Update();
			}
		}
	}
}
