using System.Text;
using Timberborn.Common;
using Timberborn.Localization;

namespace Timberborn.KeyBindingSystemUI;

public class KeyBindingTooltipFactory
{
	private static readonly string ToggleLocKey = "KeyBinding.Toggle";

	private static readonly string HoldLocKey = "KeyBinding.Hold";

	private readonly ILoc _loc;

	private readonly KeyBindingDescriber _keyBindingDescriber;

	private readonly StringBuilder _tooltip = new StringBuilder();

	public KeyBindingTooltipFactory(ILoc loc, KeyBindingDescriber keyBindingDescriber)
	{
		_loc = loc;
		_keyBindingDescriber = keyBindingDescriber;
	}

	public string Create(string headerLocKey, string toggleBindingKey, string holdBindingKey)
	{
		_tooltip.AppendLine(_loc.T(headerLocKey));
		AddKeyBindingInfo(toggleBindingKey, ToggleLocKey);
		AddKeyBindingInfo(holdBindingKey, HoldLocKey);
		return _tooltip.ToStringWithoutNewLineEndAndClean();
	}

	private void AddKeyBindingInfo(string keyBindingKey, string actionTypeLocKey)
	{
		if (_keyBindingDescriber.TryGetKeyBindingText(keyBindingKey, out var keyBindingText))
		{
			_tooltip.AppendLine(_loc.T(actionTypeLocKey) + " " + keyBindingText);
		}
	}
}
