using System;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.BatchControl;

public class ToggleButtonBatchControlRowItemFactory
{
	private static readonly string ToggleStateOnLocKey = "Toggle.State.On";

	private static readonly string ToggleStateOffLocKey = "Toggle.State.Off";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	public ToggleButtonBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public IBatchControlRowItem Create(string buttonClass, Action valueSetter, Func<bool> valueGetter, string tooltipLocKey)
	{
		string elementName = "Game/BatchControl/ToggleButtonBatchControlRowItem";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		Button button = visualElement.Q<Button>("ToggleButtonBatchControlRowItem");
		button.AddToClassList(buttonClass);
		button.RegisterCallback<ClickEvent>(delegate
		{
			valueSetter();
		});
		_tooltipRegistrar.Register(button, () => GetTooltipText(tooltipLocKey, valueGetter));
		return new ToggleButtonBatchControlRowItem(visualElement, button, valueGetter);
	}

	private string GetTooltipText(string tooltipLocKey, Func<bool> getValue)
	{
		string text = (getValue() ? _loc.T(ToggleStateOnLocKey) : _loc.T(ToggleStateOffLocKey));
		return _loc.T(tooltipLocKey) + ": " + text;
	}
}
