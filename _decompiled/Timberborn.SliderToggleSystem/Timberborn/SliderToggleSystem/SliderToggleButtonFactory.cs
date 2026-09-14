using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.SliderToggleSystem;

public class SliderToggleButtonFactory
{
	private static readonly string ToggleSelectedLocKey = "Toggle.Selected";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	public SliderToggleButtonFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public SliderToggleButton Create(VisualElement parent, SliderToggleItem sliderToggleItem)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/SliderToggleButton");
		Image image = visualElement.Q<Image>("Icon");
		if (sliderToggleItem.Icon != null)
		{
			image.sprite = sliderToggleItem.Icon;
		}
		else
		{
			image.AddToClassList(sliderToggleItem.IconClass);
		}
		Button button = visualElement.Q<Button>("Button");
		if (sliderToggleItem.SelectedClass != null)
		{
			button.AddToClassList(sliderToggleItem.SelectedClass);
		}
		button.RegisterCallback<ClickEvent>(delegate
		{
			sliderToggleItem.ClickAction();
		});
		_tooltipRegistrar.Register(button, () => GetTooltip(sliderToggleItem));
		parent.Add(visualElement);
		return new SliderToggleButton(button, sliderToggleItem.StateGetter, sliderToggleItem.ClickAction);
	}

	private TooltipContent GetTooltip(SliderToggleItem sliderToggleItem)
	{
		TooltipContent content = sliderToggleItem.TooltipGetter();
		if (content.VisualElement != null)
		{
			return content;
		}
		bool flag = sliderToggleItem.StateGetter() == SliderToggleState.Active;
		string suffix = (flag ? (" " + _loc.T(ToggleSelectedLocKey)) : string.Empty);
		return TooltipContent.Create(() => content.BaseText + suffix);
	}
}
