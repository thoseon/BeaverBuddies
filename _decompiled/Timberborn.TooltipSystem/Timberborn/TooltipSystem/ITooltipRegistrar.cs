using System;
using UnityEngine.UIElements;

namespace Timberborn.TooltipSystem;

public interface ITooltipRegistrar
{
	void RegisterLocalizable(VisualElement visualElement, string tooltipTextLocKey);

	void RegisterLocalizable(VisualElement visualElement, Func<string> tooltipTextLocKeyGetter);

	void RegisterUpdatable(VisualElement visualElement, Func<string> tooltipTextGetter);

	void RegisterInstantUpdatable(VisualElement visualElement, Func<VisualElement> tooltipTextGetter);

	void Register(VisualElement visualElement, string tooltipText);

	void Register(VisualElement visualElement, Func<string> tooltipTextGetter);

	void Register(VisualElement visualElement, VisualElement tooltipElement);

	void Register(VisualElement visualElement, Func<VisualElement> tooltipElementGetter);

	void Register(VisualElement visualElement, Func<TooltipContent> tooltipContentGetter);

	void RegisterWithKeyBinding(VisualElement visualElement, string keyBinding);

	void RegisterWithKeyBinding(VisualElement visualElement, string tooltipText, string keyBinding);

	void RegisterWithKeyBinding(VisualElement visualElement, Func<string> tooltipTextGetter, Func<string> keyBindingGetter);

	void ShowPriority(VisualElement visualElement);

	void HidePriority();
}
