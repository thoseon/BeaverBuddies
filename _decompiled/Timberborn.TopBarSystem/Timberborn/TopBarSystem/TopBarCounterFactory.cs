using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.Goods;
using Timberborn.Localization;
using Timberborn.ResourceCountingSystemUI;
using Timberborn.StockpilesUI;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.TopBarSystem;

internal class TopBarCounterFactory
{
	private static readonly string HiddenClass = "extension-clamp--hidden";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly TopBarCounterRowFactory _topBarCounterRowFactory;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	private readonly ContextualResourceCountingService _contextualResourceCountingService;

	private readonly GoodStockpilesTooltipFactory _goodStockpilesTooltipFactory;

	public TopBarCounterFactory(VisualElementLoader visualElementLoader, TopBarCounterRowFactory topBarCounterRowFactory, ITooltipRegistrar tooltipRegistrar, ILoc loc, ContextualResourceCountingService contextualResourceCountingService, GoodStockpilesTooltipFactory goodStockpilesTooltipFactory)
	{
		_visualElementLoader = visualElementLoader;
		_topBarCounterRowFactory = topBarCounterRowFactory;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
		_contextualResourceCountingService = contextualResourceCountingService;
		_goodStockpilesTooltipFactory = goodStockpilesTooltipFactory;
	}

	public ITopBarCounter CreateSimpleCounter(GoodGroupSpec goodGroupSpec, string goodId, VisualElement root)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/TopBar/SimpleTopBarCounter");
		visualElement.Q<Image>("Icon").sprite = goodGroupSpec.Icon.Asset;
		_tooltipRegistrar.Register(visualElement.Q<VisualElement>("CounterWrapper"), () => _goodStockpilesTooltipFactory.Create(goodId));
		ConfigureForDistrictMode(visualElement);
		root.Add(visualElement);
		return new TopBarCounterRow(_loc, _contextualResourceCountingService, goodId, visualElement, visualElement.Q<Label>("Count"), visualElement.Q<VisualElement>("Fill"), alwaysVisible: true);
	}

	public ITopBarCounter CreateExtendableCounter(GoodGroupSpec goodGroupSpec, IEnumerable<string> goods, VisualElement root)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/TopBar/ExtendableTopBarCounter");
		visualElement.Q<Image>("Icon").sprite = goodGroupSpec.Icon.Asset;
		_tooltipRegistrar.Register(visualElement.Q<VisualElement>("CounterWrapper"), goodGroupSpec.DisplayName.Value);
		VisualElement visualElement2 = visualElement.Q<VisualElement>("CounterItems");
		ConfigureForDistrictMode(visualElement);
		ConfigureVisibilityToggling(visualElement, visualElement2);
		root.Add(visualElement);
		return new ExtendableTopBarCounter(_loc, _topBarCounterRowFactory.Create(goods, visualElement2), visualElement.Q<Label>("EmptyCounterPlaceholder"), visualElement.Q<Label>("Count"));
	}

	private static void ConfigureForDistrictMode(VisualElement counter)
	{
		counter.Q<VisualElement>(null, "top-bar-counter").AddToClassList("top-bar-counter__wrapper--district");
	}

	private static void ConfigureVisibilityToggling(VisualElement root, VisualElement items)
	{
		Button toggler = root.Q<Button>("ExtensionToggler");
		VisualElement background = root.Q<VisualElement>("Background");
		root.Q<VisualElement>("CounterWrapper").RegisterCallback<ClickEvent>(delegate
		{
			ToggleVisibility(toggler, items, background);
		});
		toggler.RegisterCallback<ClickEvent>(delegate
		{
			ToggleVisibility(toggler, items, background);
		});
	}

	private static void ToggleVisibility(Button toggler, VisualElement items, VisualElement background)
	{
		bool flag = items.IsDisplayed();
		toggler.EnableInClassList(HiddenClass, flag);
		background.ToggleDisplayStyle(!flag);
		items.ToggleDisplayStyle(!flag);
	}
}
