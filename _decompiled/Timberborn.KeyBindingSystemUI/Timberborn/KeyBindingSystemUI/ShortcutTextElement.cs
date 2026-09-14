using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.KeyBindingSystemUI;

public class ShortcutTextElement
{
	private readonly TextElement _textElement;

	private readonly bool _alwaysVisible;

	public ShortcutTextElement(TextElement textElement, bool alwaysVisible)
	{
		_textElement = textElement;
		_alwaysVisible = alwaysVisible;
	}

	public void SetShortcut(string shortcut)
	{
		_textElement.text = shortcut;
		_textElement.ToggleDisplayStyle(visible: true);
	}

	public void SetUndefinedShortcut()
	{
		_textElement.text = string.Empty;
		_textElement.ToggleDisplayStyle(_alwaysVisible);
	}
}
