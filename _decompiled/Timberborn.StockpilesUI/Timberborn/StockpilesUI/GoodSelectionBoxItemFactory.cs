using System;
using Timberborn.CoreUI;
using Timberborn.ResourceCountingSystemUI;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class GoodSelectionBoxItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly StockpileOptionsService _stockpileOptionsService;

	private readonly ContextualResourceCountingService _contextualResourceCountingService;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly GoodStockpilesTooltipFactory _goodStockpilesTooltipFactory;

	public GoodSelectionBoxItemFactory(VisualElementLoader visualElementLoader, StockpileOptionsService stockpileOptionsService, ContextualResourceCountingService contextualResourceCountingService, ITooltipRegistrar tooltipRegistrar, GoodStockpilesTooltipFactory goodStockpilesTooltipFactory)
	{
		_visualElementLoader = visualElementLoader;
		_stockpileOptionsService = stockpileOptionsService;
		_contextualResourceCountingService = contextualResourceCountingService;
		_tooltipRegistrar = tooltipRegistrar;
		_goodStockpilesTooltipFactory = goodStockpilesTooltipFactory;
	}

	public GoodSelectionBoxItem CreateForGood(string goodId, Action<string> itemAction)
	{
		string elementName = "Game/EntityPanel/StockpileInventoryFragmentItem";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		visualElement.Q<Button>("GoodSelectionBoxItem").RegisterCallback<ClickEvent>(delegate
		{
			itemAction(goodId);
		});
		visualElement.Q<Image>("Icon").sprite = _stockpileOptionsService.GetItemIcon(goodId);
		_tooltipRegistrar.Register(visualElement, () => _goodStockpilesTooltipFactory.Create(goodId));
		return new GoodSelectionBoxItem(_contextualResourceCountingService, visualElement, goodId, visualElement.Q<VisualElement>("Fill"));
	}
}
