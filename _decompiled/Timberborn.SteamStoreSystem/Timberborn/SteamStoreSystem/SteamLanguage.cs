using Steamworks;
using Timberborn.Localization;

namespace Timberborn.SteamStoreSystem;

internal class SteamLanguage
{
	private readonly SteamManager _steamManager;

	public SteamLanguage(SteamManager steamManager)
	{
		_steamManager = steamManager;
	}

	public string GetLanguageCode()
	{
		if (!_steamManager.Initialized)
		{
			return LocalizationCodes.Default;
		}
		return SteamApps.GetCurrentGameLanguage() switch
		{
			"schinese" => LocalizationCodes.SimplifiedChinese, 
			"english" => LocalizationCodes.English, 
			"french" => LocalizationCodes.French, 
			"german" => LocalizationCodes.German, 
			"italian" => LocalizationCodes.Italian, 
			"japanese" => LocalizationCodes.Japanese, 
			"koreana" => LocalizationCodes.Korean, 
			"polish" => LocalizationCodes.Polish, 
			"brazilian" => LocalizationCodes.BrazilianPortuguese, 
			"russian" => LocalizationCodes.Russian, 
			"spanish" => LocalizationCodes.Spanish, 
			"ukrainian" => LocalizationCodes.Ukrainian, 
			"turkish" => LocalizationCodes.Turkish, 
			"thai" => LocalizationCodes.Thai, 
			"tchinese" => LocalizationCodes.TraditionalChinese, 
			_ => LocalizationCodes.Default, 
		};
	}
}
