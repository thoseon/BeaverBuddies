using Timberborn.KeyBindingSystem;
using UnityEngine.UIElements;

namespace Timberborn.KeyBindingSystemUI;

public class KeyBindingGroup
{
	private readonly KeyBindingGroupSpec _keyBindingGroupSpec;

	public VisualElement Root { get; }

	public bool IsHidden => _keyBindingGroupSpec.IsHiddenGroup;

	public KeyBindingGroup(VisualElement root, KeyBindingGroupSpec keyBindingGroupSpec)
	{
		_keyBindingGroupSpec = keyBindingGroupSpec;
		Root = root;
	}
}
