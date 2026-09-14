using System.Collections.Generic;
using Timberborn.ExperimentalModeSystem;

namespace Timberborn.Localization;

internal class LocalizationService : ILocalizationService
{
	private readonly LocalizationLoader _localizationLoader;

	private readonly LocalizationDisplayNames _localizationDisplayNames;

	private readonly ILoc _loc;

	private readonly ExperimentalMode _experimentalMode;

	private readonly PanelTextSettingsUpdater _panelTextSettingsUpdater;

	public string CurrentLanguage { get; private set; }

	public IEnumerable<LanguageInfo> AvailableLanguages => _localizationDisplayNames.GetLocalizationDisplayNames();

	public LocalizationService(LocalizationLoader localizationLoader, LocalizationDisplayNames localizationDisplayNames, ILoc loc, ExperimentalMode experimentalMode, PanelTextSettingsUpdater panelTextSettingsUpdater)
	{
		_localizationLoader = localizationLoader;
		_localizationDisplayNames = localizationDisplayNames;
		_loc = loc;
		_experimentalMode = experimentalMode;
		_panelTextSettingsUpdater = panelTextSettingsUpdater;
	}

	public void Load(string localizationCode)
	{
		CurrentLanguage = localizationCode;
		bool isExperimental = _experimentalMode.IsExperimental;
		Dictionary<string, string> localization = _localizationLoader.GetLocalization(CurrentLanguage, isExperimental);
		_loc.Initialize(localization);
		_panelTextSettingsUpdater.Update(CurrentLanguage);
	}
}
