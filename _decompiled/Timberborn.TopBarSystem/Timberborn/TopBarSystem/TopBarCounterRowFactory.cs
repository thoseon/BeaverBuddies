using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.GoodsUI;
using Timberborn.Localization;
using Timberborn.ResourceCountingSystemUI;
using Timberborn.StockpilesUI;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.TopBarSystem;

internal class TopBarCounterRowFactory
{
	private readonly GoodDescriber _goodDescriber;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ContextualResourceCountingService _contextualResourceCountingService;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	private readonly GoodStockpilesTooltipFactory _goodStockpilesTooltipFactory;

	public TopBarCounterRowFactory(GoodDescriber goodDescriber, VisualElementLoader visualElementLoader, ContextualResourceCountingService contextualResourceCountingService, ITooltipRegistrar tooltipRegistrar, ILoc loc, GoodStockpilesTooltipFactory goodStockpilesTooltipFactory)
	{
		_goodDescriber = goodDescriber;
		_visualElementLoader = visualElementLoader;
		_contextualResourceCountingService = contextualResourceCountingService;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
		_goodStockpilesTooltipFactory = goodStockpilesTooltipFactory;
	}

	public IEnumerable<TopBarCounterRow> Create(IEnumerable<string> goods, VisualElement root)
	{
		foreach (string good in goods)
		{
			yield return Create(good, root);
		}
	}

	private TopBarCounterRow Create(string goodId, VisualElement root)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/TopBar/TopBarCounterRow");
		visualElement.Q<Image>("Icon").sprite = _goodDescriber.GetIcon(goodId);
		_tooltipRegistrar.Register(visualElement, () => _goodStockpilesTooltipFactory.Create(goodId));
		root.Add(visualElement);
		return new TopBarCounterRow(_loc, _contextualResourceCountingService, goodId, visualElement, visualElement.Q<Label>("Count"), visualElement.Q<VisualElement>("Fill"));
	}
}
