using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

internal class ButtonClickabilityInitializer : IVisualElementInitializer
{
	private static readonly List<EventModifiers> EventModifiers = new List<EventModifiers>(Enum.GetValues(typeof(EventModifiers)).Cast<EventModifiers>());

	public void InitializeVisualElement(VisualElement visualElement)
	{
		if (visualElement is Button button)
		{
			MakeButtonClickableWithAnyModifier(button);
		}
	}

	private static void MakeButtonClickableWithAnyModifier(Button button)
	{
		List<ManipulatorActivationFilter> activators = button.clickable.activators;
		activators.Clear();
		foreach (EventModifiers eventModifier in EventModifiers)
		{
			activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse,
				modifiers = eventModifier
			});
		}
	}
}
