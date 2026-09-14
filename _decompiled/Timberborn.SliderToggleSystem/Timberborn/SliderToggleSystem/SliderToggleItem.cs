using System;
using Timberborn.TooltipSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.SliderToggleSystem;

public readonly struct SliderToggleItem
{
	public Func<TooltipContent> TooltipGetter { get; }

	public string IconClass { get; }

	public Sprite Icon { get; }

	public string SelectedClass { get; }

	public Action ClickAction { get; }

	public Func<SliderToggleState> StateGetter { get; }

	private SliderToggleItem(Func<TooltipContent> tooltipGetter, string iconClass, Sprite icon, string selectedClass, Action clickAction, Func<SliderToggleState> stateGetter)
	{
		TooltipGetter = tooltipGetter;
		IconClass = iconClass;
		Icon = icon;
		SelectedClass = selectedClass;
		ClickAction = clickAction;
		StateGetter = stateGetter;
	}

	public static SliderToggleItem CreateBlockable(Func<TooltipContent> tooltipGetter, string iconClass, Action clickAction, Func<SliderToggleState> stateGetter)
	{
		return new SliderToggleItem(tooltipGetter, iconClass, null, null, clickAction, stateGetter);
	}

	public static SliderToggleItem Create(Func<string> tooltipGetter, string iconClass, Action clickAction, Func<bool> isActiveGetter)
	{
		return new SliderToggleItem(() => TooltipContent.Create(tooltipGetter), iconClass, null, null, clickAction, () => ConvertActiveState(isActiveGetter()));
	}

	public static SliderToggleItem Create(Func<VisualElement> tooltipGetter, string iconClass, string selectedClass, Action clickAction, Func<bool> isActiveGetter)
	{
		return new SliderToggleItem(() => TooltipContent.Create(tooltipGetter), iconClass, null, selectedClass, clickAction, () => ConvertActiveState(isActiveGetter()));
	}

	public static SliderToggleItem Create(Func<string> tooltipGetter, Sprite icon, Action clickAction, Func<bool> isActiveGetter)
	{
		return new SliderToggleItem(() => TooltipContent.Create(tooltipGetter), null, icon, null, clickAction, () => ConvertActiveState(isActiveGetter()));
	}

	private static SliderToggleState ConvertActiveState(bool isActive)
	{
		if (!isActive)
		{
			return SliderToggleState.None;
		}
		return SliderToggleState.Active;
	}
}
