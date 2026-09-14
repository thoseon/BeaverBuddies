using Timberborn.UISound;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class UISoundInitializer : IVisualElementInitializer
{
	private static readonly CustomStyleProperty<string> ClickSoundProperty = new CustomStyleProperty<string>("--click-sound");

	private static readonly string NoSoundValue = "none";

	private readonly UISoundController _uiSoundController;

	public UISoundInitializer(UISoundController uiSoundController)
	{
		_uiSoundController = uiSoundController;
	}

	public void InitializeVisualElement(VisualElement visualElement)
	{
		visualElement.RegisterCallback<ClickEvent>(PlayUISound);
	}

	private void PlayUISound(EventBase clickEvent)
	{
		if (clickEvent.currentTarget is VisualElement { enabledSelf: not false } visualElement && clickEvent.target is VisualElement { enabledSelf: not false } visualElement2 && visualElement == visualElement2 && visualElement.customStyle.TryGetValue(ClickSoundProperty, out var value) && value != NoSoundValue)
		{
			_uiSoundController.PlaySound(value);
		}
	}
}
