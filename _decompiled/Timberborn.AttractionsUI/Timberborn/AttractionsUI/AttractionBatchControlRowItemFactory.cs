using Timberborn.Attractions;
using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EnterableSystem;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.AttractionsUI;

public class AttractionBatchControlRowItemFactory
{
	private static readonly string VisitorsLocKey = "Attractions.Visitors";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	public AttractionBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		Attraction component = entity.GetComponent<Attraction>();
		if (component != null)
		{
			string elementName = "Game/BatchControl/AttractionBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Label capacityLabel = visualElement.Q<Label>("Info");
			Enterable component2 = component.GetComponent<Enterable>();
			_tooltipRegistrar.RegisterLocalizable(visualElement, VisitorsLocKey);
			return new AttractionBatchControlRowItem(visualElement, capacityLabel, component2);
		}
		return null;
	}
}
