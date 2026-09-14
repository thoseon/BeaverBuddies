using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.Language;

internal class LanguageLoader : ILoadableSingleton
{
	private readonly LanguageSettings _languageSettings;

	private readonly ILocalizationService _localizationService;

	public LanguageLoader(LanguageSettings languageSettings, ILocalizationService localizationService)
	{
		_languageSettings = languageSettings;
		_localizationService = localizationService;
	}

	public void Load()
	{
		_localizationService.Load(_languageSettings.Language);
	}
}
