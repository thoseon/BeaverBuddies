using Timberborn.CoreUI;
using Timberborn.NeedSpecs;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

internal class NeedGroupViewFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	public NeedGroupViewFactory(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public NeedGroupView Create(NeedGroupSpec needGroupSpec)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityPanel/NeedGroupView");
		visualElement.Q<Label>("Label").text = needGroupSpec.DisplayName.Value;
		VisualElement items = visualElement.Q<VisualElement>("Items");
		return new NeedGroupView(visualElement, items);
	}
}
