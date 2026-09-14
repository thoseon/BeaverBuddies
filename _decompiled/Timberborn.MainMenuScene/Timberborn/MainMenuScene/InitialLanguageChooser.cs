using System;
using System.Linq;
using Timberborn.LanguageUI;
using Timberborn.Localization;

namespace Timberborn.MainMenuScene;

internal class InitialLanguageChooser
{
	private readonly ChangeLanguageBox _changeLanguageBox;

	private readonly ILocalizationService _localizationService;

	private bool NewLanguagesDetected => _localizationService.AvailableLanguages.Any((LanguageInfo language) => language.IsNew);

	private bool CurrentLanguageIsMissing => _localizationService.AvailableLanguages.All((LanguageInfo language) => language.LocalizationCode != _localizationService.CurrentLanguage);

	public InitialLanguageChooser(ChangeLanguageBox changeLanguageBox, ILocalizationService localizationService)
	{
		_changeLanguageBox = changeLanguageBox;
		_localizationService = localizationService;
	}

	public void CheckInitialLanguage(Action onSuccessfulCheck)
	{
		if (NewLanguagesDetected || CurrentLanguageIsMissing)
		{
			_changeLanguageBox.ShowWithoutReloadConfirmation(onSuccessfulCheck);
		}
		else
		{
			onSuccessfulCheck();
		}
	}
}
