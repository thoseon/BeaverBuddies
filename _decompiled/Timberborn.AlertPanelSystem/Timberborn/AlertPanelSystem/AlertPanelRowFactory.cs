using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.StatusSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.AlertPanelSystem;

public class AlertPanelRowFactory
{
	private static readonly string EnableHoverClass = "hover-enabled";

	private readonly ILoc _loc;

	private readonly StatusSpriteLoader _statusSpriteLoader;

	private readonly VisualElementLoader _visualElementLoader;

	public AlertPanelRowFactory(ILoc loc, StatusSpriteLoader statusSpriteLoader, VisualElementLoader visualElementLoader)
	{
		_loc = loc;
		_statusSpriteLoader = statusSpriteLoader;
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement CreateClosable(string statusIconName)
	{
		string elementName = "Common/AlertPanel/ClosableAlertPanelRow";
		VisualElement root = _visualElementLoader.LoadVisualElement(elementName);
		root.Q<Image>("Icon").sprite = _statusSpriteLoader.LoadSprite(statusIconName);
		Button button = root.Q<Button>("Close");
		root.AddToClassList(EnableHoverClass);
		button.RegisterCallback<MouseEnterEvent>(delegate
		{
			root.RemoveFromClassList(EnableHoverClass);
		});
		button.RegisterCallback<MouseLeaveEvent>(delegate
		{
			root.AddToClassList(EnableHoverClass);
		});
		button.RegisterCallback<ClickEvent>(delegate
		{
			root.ToggleDisplayStyle(visible: false);
		});
		root.ToggleDisplayStyle(visible: false);
		return root;
	}

	public VisualElement Create(string labelLocKey, string statusIconName)
	{
		return CreateInternal(_loc.T(labelLocKey), _statusSpriteLoader.LoadSprite(statusIconName));
	}

	public VisualElement Create(Sprite sprite)
	{
		return CreateInternal("", sprite);
	}

	private VisualElement CreateInternal(string label, Sprite sprite)
	{
		string elementName = "Common/AlertPanel/AlertPanelRow";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		visualElement.Q<Image>("Icon").sprite = sprite;
		visualElement.Q<Button>("Button").text = label;
		visualElement.ToggleDisplayStyle(visible: false);
		return visualElement;
	}
}
