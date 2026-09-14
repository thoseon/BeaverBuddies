using Timberborn.CoreUI;
using Timberborn.TooltipSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.UIFormatters;

public class DescribedAmountFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	public DescribedAmountFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public VisualElement CreatePlain(string rootClass, string amount, Sprite icon, string tooltip)
	{
		VisualElement visualElement = CreatePlain(rootClass, amount, tooltip);
		visualElement.Q<Image>("Icon").sprite = icon;
		return visualElement;
	}

	public VisualElement CreatePlain(string rootClass, string amount, string tooltip)
	{
		VisualElement visualElement = CreatePlain(rootClass, amount);
		_tooltipRegistrar.Register(visualElement, tooltip);
		return visualElement;
	}

	public VisualElement CreatePlain(string rootClass, string amount)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityDescription/DescribedAmountPlain");
		visualElement.AddToClassList(rootClass);
		visualElement.Q<Label>("Amount").text = amount;
		return visualElement;
	}

	public VisualElement CreateBordered(string amount, Sprite icon, string tooltip)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityDescription/DescribedAmountBordered");
		visualElement.Q<Label>("Amount").text = amount;
		visualElement.Q<Image>("Icon").sprite = icon;
		_tooltipRegistrar.Register(visualElement, tooltip);
		return visualElement;
	}
}
