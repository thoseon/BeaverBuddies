using UnityEngine.UIElements;

namespace Timberborn.DropdownSystem;

public readonly struct DropdownElement
{
	public VisualElement Content { get; }

	public string Tooltip { get; }

	public DropdownElement(VisualElement content, string tooltip)
	{
		Content = content;
		Tooltip = tooltip;
	}
}
