using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Localization;

internal class NewLocalizationService : ILoadableSingleton
{
	private static readonly string LocalizationsOnLastLaunchKey = "LocalizationsOnLastLaunch";

	private static readonly char Delimiter = ';';

	private readonly LocalizationLoader _localizationLoader;

	private readonly HashSet<string> _localizationsAvailableOnLastLaunch = new HashSet<string>();

	public NewLocalizationService(LocalizationLoader localizationLoader)
	{
		_localizationLoader = localizationLoader;
	}

	public void Load()
	{
		string localizations = PlayerPrefs.GetString(LocalizationsOnLastLaunchKey, "");
		_localizationsAvailableOnLastLaunch.AddRange(DeserializeLocalizations(localizations));
		string value = SerializeLocalizations(_localizationLoader.GetLocalizationNames());
		PlayerPrefs.SetString(LocalizationsOnLastLaunchKey, value);
	}

	public bool LocalizationIsNew(string localizationCode)
	{
		return !_localizationsAvailableOnLastLaunch.Contains(localizationCode);
	}

	private static string SerializeLocalizations(IEnumerable<string> localizations)
	{
		return string.Join(Delimiter, localizations.OrderBy((string localization) => localization));
	}

	private static IEnumerable<string> DeserializeLocalizations(string localizations)
	{
		return localizations.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries);
	}
}
