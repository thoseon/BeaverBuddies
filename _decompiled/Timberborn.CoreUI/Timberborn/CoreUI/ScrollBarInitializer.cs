using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class ScrollBarInitializer : IVisualElementInitializer
{
	private readonly ScrollBarInitializationService _scrollBarInitializationService;

	public ScrollBarInitializer(ScrollBarInitializationService scrollBarInitializationService)
	{
		_scrollBarInitializationService = scrollBarInitializationService;
	}

	public void InitializeVisualElement(VisualElement visualElement)
	{
		_scrollBarInitializationService.InitializeVisualElement(visualElement);
	}
}
