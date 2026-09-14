using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.KeyBindingSystem;

public class KeyBindingRegistry : ILoadableSingleton
{
	private readonly IKeyBindingBlocker _keyBindingBlocker;

	private readonly KeyBindingSpecService _keyBindingSpecService;

	private readonly KeyBindingFactory _keyBindingFactory;

	private readonly List<KeyBinding> _keyBindings = new List<KeyBinding>();

	private readonly Dictionary<string, KeyBinding> _keyBindingsById = new Dictionary<string, KeyBinding>();

	public ReadOnlyList<KeyBinding> KeyBindings => _keyBindings.AsReadOnlyList();

	public KeyBindingRegistry(IKeyBindingBlocker keyBindingBlocker, KeyBindingSpecService keyBindingSpecService, KeyBindingFactory keyBindingFactory)
	{
		_keyBindingBlocker = keyBindingBlocker;
		_keyBindingSpecService = keyBindingSpecService;
		_keyBindingFactory = keyBindingFactory;
	}

	public void Load()
	{
		foreach (KeyBindingDefinition keyBindingDefinition in _keyBindingSpecService.KeyBindingDefinitions)
		{
			Add(keyBindingDefinition);
		}
	}

	public bool IsDown(string id)
	{
		KeyBinding keyBinding = _keyBindingsById[id];
		if (!_keyBindingBlocker.IsKeyBlocked(keyBinding))
		{
			return keyBinding.IsDown;
		}
		return false;
	}

	public bool IsHeld(string id)
	{
		KeyBinding keyBinding = _keyBindingsById[id];
		if (!_keyBindingBlocker.IsKeyBlocked(keyBinding))
		{
			return keyBinding.IsHeld;
		}
		return false;
	}

	public bool IsLongHeld(string id)
	{
		KeyBinding keyBinding = _keyBindingsById[id];
		if (!_keyBindingBlocker.IsKeyBlocked(keyBinding))
		{
			return keyBinding.IsLongHeld;
		}
		return false;
	}

	public bool IsUp(string id)
	{
		KeyBinding keyBinding = _keyBindingsById[id];
		if (!_keyBindingBlocker.IsKeyBlocked(keyBinding))
		{
			return keyBinding.IsUp;
		}
		return false;
	}

	public bool IsUpAfterShortHeld(string id)
	{
		KeyBinding keyBinding = _keyBindingsById[id];
		if (!_keyBindingBlocker.IsKeyBlocked(keyBinding))
		{
			return keyBinding.IsUpAfterShortHeld;
		}
		return false;
	}

	public float GetRawValue(string id)
	{
		KeyBinding keyBinding = _keyBindingsById[id];
		if (_keyBindingBlocker.IsKeyBlocked(keyBinding))
		{
			return 0f;
		}
		return keyBinding.GetRawValue();
	}

	public KeyBinding Get(string id)
	{
		return _keyBindingsById[id];
	}

	private void Add(KeyBindingDefinition keyBindingDefinition)
	{
		KeyBinding keyBinding = _keyBindingFactory.Create(keyBindingDefinition);
		_keyBindings.Add(keyBinding);
		_keyBindingsById.Add(keyBinding.Id, keyBinding);
	}
}
