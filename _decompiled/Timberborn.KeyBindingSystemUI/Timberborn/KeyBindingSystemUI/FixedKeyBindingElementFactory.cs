using System;
using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.KeyBindingSystemUI;

public class FixedKeyBindingElementFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	public FixedKeyBindingElementFactory(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement Create(string keyBindingId)
	{
		string[] array = keyBindingId.Split('|');
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/FixedKeyBinding");
		visualElement.AddToClassList(MouseButtonToClass(array[0]));
		visualElement.AddToClassList(DirectionToClass(array[1]));
		return visualElement;
	}

	private static string MouseButtonToClass(string mouseButton)
	{
		return mouseButton switch
		{
			"MouseLeft" => "mouse-left", 
			"MouseRight" => "mouse-right", 
			"MouseMiddle" => "mouse-middle", 
			"MouseZoom" => "mouse-zoom", 
			_ => throw new ArgumentOutOfRangeException("mouseButton", mouseButton, null), 
		};
	}

	private static string DirectionToClass(string direction)
	{
		return direction switch
		{
			"Up" => "up", 
			"Down" => "down", 
			"Left" => "left", 
			"Right" => "right", 
			"ScrollUp" => "scroll-up", 
			"ScrollDown" => "scroll-down", 
			_ => throw new ArgumentOutOfRangeException("direction", direction, null), 
		};
	}
}
