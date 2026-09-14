using System.Collections.Generic;

namespace Timberborn.Localization;

public interface ILocalizationService
{
	string CurrentLanguage { get; }

	IEnumerable<LanguageInfo> AvailableLanguages { get; }

	void Load(string localizationCode);
}
