using System;
using Timberborn.InputSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public static class VisualElementExtensions
{
	public static void ToggleDisplayStyle(this VisualElement visualElement, bool visible)
	{
		visualElement.style.display = ((!visible) ? DisplayStyle.None : DisplayStyle.Flex);
	}

	public static bool IsDisplayed(this VisualElement visualElement)
	{
		return visualElement.resolvedStyle.display == DisplayStyle.Flex;
	}

	public static void SetHeightAsPercent(this VisualElement visualElement, float value01)
	{
		float value2 = Mathf.Clamp01(value01) * 100f;
		visualElement.style.height = new StyleLength(new Length(value2, LengthUnit.Percent));
	}

	public static bool IsFocused(this VisualElement visualElement)
	{
		return visualElement.focusController?.focusedElement == visualElement;
	}

	public static void SetConfirmCancelActions(this VisualElement visualElement, InputService inputService, Action confirmAction, Action cancelAction)
	{
		visualElement.RegisterCallback<FocusOutEvent>(delegate
		{
			if (inputService.WasConfirmPressedLastFrame)
			{
				confirmAction();
			}
			if (inputService.WasCancelPressedLastFrame)
			{
				cancelAction();
			}
		});
	}
}
