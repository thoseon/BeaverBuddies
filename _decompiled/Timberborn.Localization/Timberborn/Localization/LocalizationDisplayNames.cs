using System.Collections.Generic;

namespace Timberborn.Localization;

internal class LocalizationDisplayNames
{
	private static readonly string LocalizationDisplayNameKey = "Settings.Language.Name";

	private readonly LocalizationLoader _localizationLoader;

	private readonly NewLocalizationService _newLocalizationService;

	public LocalizationDisplayNames(LocalizationLoader localizationLoader, NewLocalizationService newLocalizationService)
	{
		_localizationLoader = localizationLoader;
		_newLocalizationService = newLocalizationService;
	}

	public IEnumerable<LanguageInfo> GetLocalizationDisplayNames()
	{
		foreach (string localizationName in _localizationLoader.GetLocalizationNames())
		{
			yield return GetDisplayName(localizationName);
		}
	}

	private LanguageInfo GetDisplayName(string localizationCode)
	{
		string valueOrDefault = _localizationLoader.GetLocalizationRecords(localizationCode).GetValueOrDefault(LocalizationDisplayNameKey, localizationCode);
		return new LanguageInfo(localizationCode, valueOrDefault, _newLocalizationService.LocalizationIsNew(localizationCode));
	}
}
