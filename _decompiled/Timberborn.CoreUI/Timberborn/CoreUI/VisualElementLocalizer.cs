using System;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

internal class VisualElementLocalizer : IVisualElementInitializer
{
	private readonly ILoc _loc;

	public VisualElementLocalizer(ILoc loc)
	{
		_loc = loc;
	}

	public void InitializeVisualElement(VisualElement visualElement)
	{
		if (visualElement is ILocalizableElement localizableElement)
		{
			if (!localizableElement.IsSet)
			{
				string text = ElementPath(visualElement);
				throw new InvalidOperationException("text-loc-key is not set for ILocalizableElement: " + text);
			}
			localizableElement.Localize(_loc);
		}
	}

	private static string ElementPath(VisualElement visualElement)
	{
		return ((visualElement.parent != null) ? ElementPath(visualElement.parent) : string.Empty) + "/" + (string.IsNullOrEmpty(visualElement.name) ? visualElement.GetType().Name : visualElement.name);
	}
}
