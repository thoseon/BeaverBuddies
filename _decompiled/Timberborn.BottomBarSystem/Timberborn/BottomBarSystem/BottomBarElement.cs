using UnityEngine.UIElements;

namespace Timberborn.BottomBarSystem;

public readonly struct BottomBarElement
{
	public VisualElement MainElement { get; }

	public VisualElement SubElement { get; }

	private BottomBarElement(VisualElement mainElement, VisualElement subElement)
	{
		MainElement = mainElement;
		SubElement = subElement;
	}

	public static BottomBarElement CreateMultiLevel(VisualElement mainElement, VisualElement subElement)
	{
		return new BottomBarElement(mainElement, subElement);
	}

	public static BottomBarElement CreateSingleLevel(VisualElement mainElement)
	{
		return new BottomBarElement(mainElement, null);
	}
}
