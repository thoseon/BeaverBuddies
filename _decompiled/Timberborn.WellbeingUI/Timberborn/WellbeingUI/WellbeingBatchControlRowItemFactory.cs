using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.NeedSystem;
using Timberborn.Wellbeing;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class WellbeingBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly WellbeingSummaryFactory _wellbeingSummaryFactory;

	public WellbeingBatchControlRowItemFactory(VisualElementLoader visualElementLoader, WellbeingSummaryFactory wellbeingSummaryFactory)
	{
		_visualElementLoader = visualElementLoader;
		_wellbeingSummaryFactory = wellbeingSummaryFactory;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		NeedManager component = entity.GetComponent<NeedManager>();
		if ((bool)(BaseComponent)(object)component && (bool)((BaseComponent)(object)component).GetComponent<WellbeingTracker>())
		{
			string elementName = "Game/BatchControl/WellbeingBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			WellbeingSummary wellbeingSummary = _wellbeingSummaryFactory.Create((BaseComponent)(object)component);
			visualElement.Q<VisualElement>("Summary").Add(wellbeingSummary.Root);
			return new WellbeingBatchControlRowItem(visualElement, wellbeingSummary);
		}
		return null;
	}
}
