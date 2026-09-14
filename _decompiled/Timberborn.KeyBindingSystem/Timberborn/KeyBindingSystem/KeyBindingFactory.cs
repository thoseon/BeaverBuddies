using Timberborn.Common;
using Timberborn.Localization;
using UnityEngine;

namespace Timberborn.KeyBindingSystem;

public class KeyBindingFactory
{
	private readonly KeyBindingGroupSpecService _keyBindingGroupSpecService;

	private readonly ILoc _loc;

	public KeyBindingFactory(KeyBindingGroupSpecService keyBindingGroupSpecService, ILoc loc)
	{
		_keyBindingGroupSpecService = keyBindingGroupSpecService;
		_loc = loc;
	}

	public KeyBinding Create(KeyBindingDefinition keyBindingDefinition)
	{
		KeyBindingSpec keyBindingSpec = keyBindingDefinition.KeyBindingSpec;
		bool isHidden = _keyBindingGroupSpecService.IsHiddenGroup(keyBindingSpec.GroupId);
		return new KeyBinding(GetDisplayName(keyBindingSpec, isHidden), keyBindingDefinition, isHidden);
	}

	private string GetDisplayName(KeyBindingSpec keyBindingSpec, bool isHidden)
	{
		string locKey = keyBindingSpec.LocKey;
		if (string.IsNullOrEmpty(locKey))
		{
			string id = keyBindingSpec.Id;
			if (!isHidden)
			{
				Debug.LogWarning("Loc key not defined for key binding: " + id);
			}
			return "<color=\"orange\">" + id.SplitPascalCase() + "</color>";
		}
		return _loc.T(locKey);
	}
}
