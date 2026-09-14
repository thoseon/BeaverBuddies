using System;
using Timberborn.KeyBindingSystemUI;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.TooltipSystem;

internal class TooltipRegistrar : ITooltipRegistrar
{
	private readonly Tooltip _tooltip;

	private readonly ILoc _loc;

	private readonly InputBindingDescriber _inputBindingDescriber;

	private readonly TooltipContainer _tooltipContainer;

	private readonly KeyBindingDescriber _keyBindingDescriber;

	public TooltipRegistrar(Tooltip tooltip, ILoc loc, InputBindingDescriber inputBindingDescriber, TooltipContainer tooltipContainer, KeyBindingDescriber keyBindingDescriber)
	{
		_tooltip = tooltip;
		_loc = loc;
		_inputBindingDescriber = inputBindingDescriber;
		_tooltipContainer = tooltipContainer;
		_keyBindingDescriber = keyBindingDescriber;
	}

	public void RegisterLocalizable(VisualElement visualElement, string tooltipTextLocKey)
	{
		Register(visualElement, TooltipContent.Create(() => _loc.T(tooltipTextLocKey)));
	}

	public void RegisterLocalizable(VisualElement visualElement, Func<string> tooltipTextLocKeyGetter)
	{
		Register(visualElement, TooltipContent.Create(() => _loc.T(tooltipTextLocKeyGetter())));
	}

	public void RegisterUpdatable(VisualElement visualElement, Func<string> tooltipTextGetter)
	{
		Register(visualElement, TooltipContent.CreateUpdatable(tooltipTextGetter));
	}

	public void RegisterInstantUpdatable(VisualElement visualElement, Func<VisualElement> tooltipElementGetter)
	{
		Register(visualElement, TooltipContent.CreateInstantUpdatable(tooltipElementGetter));
	}

	public void Register(VisualElement visualElement, string tooltipText)
	{
		Register(visualElement, TooltipContent.Create(() => tooltipText));
	}

	public void Register(VisualElement visualElement, Func<string> tooltipTextGetter)
	{
		Register(visualElement, TooltipContent.Create(tooltipTextGetter));
	}

	public void Register(VisualElement visualElement, VisualElement tooltipElement)
	{
		Register(visualElement, TooltipContent.Create(() => tooltipElement));
	}

	public void Register(VisualElement visualElement, Func<VisualElement> tooltipElementGetter)
	{
		Register(visualElement, TooltipContent.Create(tooltipElementGetter));
	}

	public void Register(VisualElement visualElement, Func<TooltipContent> tooltipContentGetter)
	{
		_tooltip.RegisterTooltip(visualElement, tooltipContentGetter);
	}

	public void RegisterWithKeyBinding(VisualElement visualElement, string keyBinding)
	{
		Register(visualElement, TooltipContent.CreateWithKeyBinding(GetKeyBindingDescription(keyBinding), () => GetKeyBindingText(keyBinding)));
	}

	public void RegisterWithKeyBinding(VisualElement visualElement, string tooltipText, string keyBinding)
	{
		Register(visualElement, TooltipContent.CreateWithKeyBinding(tooltipText, () => GetKeyBindingText(keyBinding)));
	}

	public void RegisterWithKeyBinding(VisualElement visualElement, Func<string> tooltipTextGetter, Func<string> keyBindingGetter)
	{
		Register(visualElement, TooltipContent.CreateWithKeyBinding(tooltipTextGetter, () => GetKeyBindingText(keyBindingGetter())));
	}

	public void ShowPriority(VisualElement visualElement)
	{
		_tooltipContainer.ShowPriority(visualElement);
	}

	public void HidePriority()
	{
		_tooltipContainer.HidePriority();
	}

	private void Register(VisualElement visualElement, TooltipContent tooltipContent)
	{
		Register(visualElement, () => tooltipContent);
	}

	private string GetKeyBindingDescription(string keyBinding)
	{
		return _inputBindingDescriber.GetKeyBindingDisplayName(keyBinding);
	}

	private string GetKeyBindingText(string keyBinding)
	{
		if (!_keyBindingDescriber.TryGetKeyBindingText(keyBinding, out var keyBindingText))
		{
			return null;
		}
		return keyBindingText;
	}
}
