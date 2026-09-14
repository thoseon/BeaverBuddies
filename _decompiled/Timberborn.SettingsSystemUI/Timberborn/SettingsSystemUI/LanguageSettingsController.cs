using Timberborn.LanguageUI;
using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class LanguageSettingsController
{
	private readonly ChangeLanguageBox _changeLanguageBox;

	public LanguageSettingsController(ChangeLanguageBox changeLanguageBox)
	{
		_changeLanguageBox = changeLanguageBox;
	}

	public void Initialize(VisualElement root)
	{
		root.Q<Label>("LanguageName").text = _changeLanguageBox.LocalizedCurrentLanguageName;
		root.Q<Button>("LanguageChange").RegisterCallback<ClickEvent>(delegate
		{
			_changeLanguageBox.ShowWithReloadConfirmation();
		});
	}
}
