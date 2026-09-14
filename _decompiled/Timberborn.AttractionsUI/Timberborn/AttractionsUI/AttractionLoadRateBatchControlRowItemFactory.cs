using System.Collections.Generic;
using Timberborn.Attractions;
using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.TimeSystem;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.AttractionsUI;

public class AttractionLoadRateBatchControlRowItemFactory
{
	private static readonly string LoadRateLocKey = "Attractions.LoadRate";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly IDayNightCycle _dayNightCycle;

	public AttractionLoadRateBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, IDayNightCycle dayNightCycle)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_dayNightCycle = dayNightCycle;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		AttractionLoadRate component = entity.GetComponent<AttractionLoadRate>();
		if (component != null)
		{
			string elementName = "Game/BatchControl/AttractionLoadRateBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			_tooltipRegistrar.RegisterLocalizable(visualElement, LoadRateLocKey);
			IEnumerable<VisualElement> loadRateRoots = CreateLoadRates(visualElement);
			AttractionLoadRateBatchControlRowItem attractionLoadRateBatchControlRowItem = new AttractionLoadRateBatchControlRowItem(_dayNightCycle, visualElement, component, loadRateRoots);
			attractionLoadRateBatchControlRowItem.Initialize();
			return attractionLoadRateBatchControlRowItem;
		}
		return null;
	}

	private IEnumerable<VisualElement> CreateLoadRates(VisualElement root)
	{
		for (int i = 0; i < 24; i++)
		{
			VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/AttractionLoadRate");
			root.Add(visualElement);
			yield return visualElement;
		}
	}
}
