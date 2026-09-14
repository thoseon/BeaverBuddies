using Timberborn.PlatformUtilities;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class ScrollBarInitializationService
{
	private static readonly string VerticalDraggerClass = "vertical-scroll-view__nine-slice-dragger";

	private static readonly string VerticalTrackerClass = "vertical-scroll-view__nine-slice-tracker";

	private static readonly string HorizontalDraggerClass = "horizontal-scroll-view__nine-slice-dragger";

	private static readonly string HorizontalTrackerClass = "horizontal-scroll-view__nine-slice-tracker";

	public void InitializeVisualElement(VisualElement visualElement)
	{
		if (visualElement is ScrollView scrollView)
		{
			SetScrollView(scrollView);
		}
	}

	private static void SetScrollView(ScrollView scrollView)
	{
		AddNineSliceElements(scrollView.verticalScroller.slider, VerticalDraggerClass, VerticalTrackerClass);
		AddNineSliceElements(scrollView.horizontalScroller.slider, HorizontalDraggerClass, HorizontalTrackerClass);
		scrollView.mouseWheelScrollSize = ScrollWheelSpeed.WheelScrollSize.Value;
	}

	private static void AddNineSliceElements(VisualElement root, string draggerClass, string trackerClass)
	{
		VisualElement visualElement = root.Q<VisualElement>("unity-dragger");
		NineSliceVisualElement nineSliceVisualElement = new NineSliceVisualElement();
		nineSliceVisualElement.AddToClassList(draggerClass);
		visualElement.Add(nineSliceVisualElement);
		VisualElement visualElement2 = root.Q<VisualElement>("unity-tracker");
		NineSliceVisualElement nineSliceVisualElement2 = new NineSliceVisualElement();
		nineSliceVisualElement2.AddToClassList(trackerClass);
		visualElement2.Add(nineSliceVisualElement2);
	}
}
