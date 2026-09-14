using Timberborn.CoreUI;
using Timberborn.Gathering;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using Timberborn.Yielding;
using Timberborn.YieldingUI;
using UnityEngine.UIElements;

namespace Timberborn.GatheringUI;

public class GatherableToolPanelItemFactory
{
	private static readonly string GrowsWhenMatureLocKey = "Growing.GrowsWhenMature";

	private static readonly string IconClass = "resource-yield__icon--calendar-cycle";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly YieldTooltipFactory _yieldTooltipFactory;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly GoodDescriber _goodDescriber;

	private readonly Phrase _growthTimePhrase = Phrase.New().FormatDays<float>("0.#");

	public GatherableToolPanelItemFactory(VisualElementLoader visualElementLoader, ILoc loc, YieldTooltipFactory yieldTooltipFactory, ITooltipRegistrar tooltipRegistrar, GoodDescriber goodDescriber)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_yieldTooltipFactory = yieldTooltipFactory;
		_tooltipRegistrar = tooltipRegistrar;
		_goodDescriber = goodDescriber;
	}

	public VisualElement Create(GatherableSpec gatherableSpec)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/ToolPanel/ResourceYieldPanelItem");
		string text = _loc.T(_growthTimePhrase, gatherableSpec.YieldGrowthTimeInDays);
		visualElement.Q<Label>("GrowthTime").text = text;
		YielderSpec yielder = gatherableSpec.Yielder;
		GoodAmountSpec yield = yielder.Yield;
		visualElement.Q<Label>("YieldAmount").text = yield.Amount.ToString();
		visualElement.Q<Image>("YieldIcon").sprite = _goodDescriber.GetIcon(yield.Id);
		visualElement.Q<VisualElement>("Calendar").AddToClassList(IconClass);
		_tooltipRegistrar.Register(visualElement, _yieldTooltipFactory.Create(yielder, text, _loc.T(GrowsWhenMatureLocKey)));
		return visualElement;
	}
}
