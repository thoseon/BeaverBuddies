using Timberborn.LanguageUI;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.TitleScreenUI;

internal class ChangeLanguageButtonInitializer : ILoadableSingleton
{
	private readonly TitleScreenFooter _titleScreenFooter;

	private readonly ChangeLanguageBox _changeLanguageBox;

	public ChangeLanguageButtonInitializer(TitleScreenFooter titleScreenFooter, ChangeLanguageBox changeLanguageBox)
	{
		_titleScreenFooter = titleScreenFooter;
		_changeLanguageBox = changeLanguageBox;
	}

	public void Load()
	{
		Button button = _titleScreenFooter.Root.Q<Button>("ChangeLanguageButton");
		button.text = _changeLanguageBox.LocalizedCurrentLanguageName;
		button.RegisterCallback<ClickEvent>(ChangeLanguageClicked);
	}

	private void ChangeLanguageClicked(ClickEvent evt)
	{
		_changeLanguageBox.ShowWithoutReloadConfirmation();
	}
}
