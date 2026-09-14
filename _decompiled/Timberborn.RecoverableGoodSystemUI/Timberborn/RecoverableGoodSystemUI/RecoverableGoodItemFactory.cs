using Timberborn.CoreUI;
using Timberborn.Effects;
using Timberborn.GoodsUI;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.RecoverableGoodSystemUI;

public class RecoverableGoodItemFactory
{
	private readonly GoodDescriber _goodDescriber;

	private readonly GoodEffectDescriber _goodEffectDescriber;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	public RecoverableGoodItemFactory(GoodDescriber goodDescriber, GoodEffectDescriber goodEffectDescriber, ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader)
	{
		_goodDescriber = goodDescriber;
		_goodEffectDescriber = goodEffectDescriber;
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
	}

	public RecoverableGoodItem Create(string goodId)
	{
		string elementName = "Game/RecoverableGood/RecoverableGoodItem";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		string tooltipText = _goodEffectDescriber.DescribeEffectsWithHeader(goodId);
		_tooltipRegistrar.Register(visualElement, tooltipText);
		DescribedGood describedGood = _goodDescriber.GetDescribedGood(goodId);
		visualElement.Q<Image>("GoodIcon").sprite = describedGood.Icon;
		Label amountLabel = visualElement.Q<Label>("Amount");
		return new RecoverableGoodItem(visualElement, goodId, amountLabel);
	}
}
