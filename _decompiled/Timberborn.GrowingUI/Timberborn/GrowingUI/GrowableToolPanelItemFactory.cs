using Timberborn.CoreUI;
using Timberborn.Cutting;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.Growing;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using Timberborn.Yielding;
using Timberborn.YieldingUI;
using UnityEngine.UIElements;

namespace Timberborn.GrowingUI;

public class GrowableToolPanelItemFactory
{
	private static readonly string GrowingTimeLocKey = "Growing.Time";

	private static readonly string IconClass = "resource-yield__icon--calendar";

	private static readonly string NoYieldClass = "resource-yield__no-yield";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly YieldTooltipFactory _yieldTooltipFactory;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly GoodDescriber _goodDescriber;

	private readonly Phrase _growthTimePhrase = Phrase.New().FormatDays<float>("0.#");

	public GrowableToolPanelItemFactory(VisualElementLoader visualElementLoader, ILoc loc, YieldTooltipFactory yieldTooltipFactory, ITooltipRegistrar tooltipRegistrar, GoodDescriber goodDescriber)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_yieldTooltipFactory = yieldTooltipFactory;
		_tooltipRegistrar = tooltipRegistrar;
		_goodDescriber = goodDescriber;
	}

	public VisualElement Create(GrowableSpec growableSpec)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/ToolPanel/ResourceYieldPanelItem");
		string text = _loc.T(_growthTimePhrase, growableSpec.GrowthTimeInDays);
		visualElement.Q<Label>("GrowthTime").text = text;
		Label label = visualElement.Q<Label>("YieldAmount");
		Image image = visualElement.Q<Image>("YieldIcon");
		CuttableSpec spec = growableSpec.GetSpec<CuttableSpec>();
		if (spec != null)
		{
			YielderSpec yielder = spec.Yielder;
			GoodAmountSpec yield = yielder.Yield;
			label.text = yield.Amount.ToString();
			image.sprite = _goodDescriber.GetIcon(yield.Id);
			_tooltipRegistrar.Register(visualElement, _yieldTooltipFactory.Create(yielder, text));
		}
		else
		{
			_tooltipRegistrar.Register(visualElement, _loc.T(GrowingTimeLocKey, text));
		}
		label.ToggleDisplayStyle(spec != null);
		image.EnableInClassList(NoYieldClass, spec == null);
		visualElement.Q<VisualElement>("Calendar").AddToClassList(IconClass);
		return visualElement;
	}
}
