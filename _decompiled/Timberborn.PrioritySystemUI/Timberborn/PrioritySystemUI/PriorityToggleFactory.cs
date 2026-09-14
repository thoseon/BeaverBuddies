using Timberborn.CoreUI;
using Timberborn.PrioritySystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.PrioritySystemUI;

public class PriorityToggleFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly PriorityColors _priorityColors;

	public PriorityToggleFactory(VisualElementLoader visualElementLoader, PriorityColors priorityColors)
	{
		_visualElementLoader = visualElementLoader;
		_priorityColors = priorityColors;
	}

	public PriorityToggle Create(Priority priority, VisualElement parent, Sprite sprite)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityPanel/PriorityToggle");
		parent.Add(visualElement);
		Color buttonColor = _priorityColors.GetButtonColor(priority);
		Toggle toggle = visualElement.Q<Toggle>("PriorityToggle");
		VisualElement visualElement2 = toggle.Q<VisualElement>("unity-checkmark");
		visualElement2.style.unityBackgroundImageTintColor = new StyleColor(buttonColor);
		visualElement2.style.backgroundImage = new StyleBackground(sprite);
		toggle.style.unityBackgroundImageTintColor = new StyleColor(buttonColor);
		PriorityToggle priorityToggle = new PriorityToggle(priority, toggle);
		priorityToggle.Initialize();
		return priorityToggle;
	}
}
