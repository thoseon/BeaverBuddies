using System;
using Timberborn.Localization;
using Timberborn.StoreSystem;

namespace Timberborn.SteamStoreSystem;

internal class SteamStore : IStore
{
	private static readonly string SteamCompatibilityMessageLocKey = "Saving.SteamCompatibilityMessage";

	private readonly SteamManager _steamManager;

	private readonly SteamLanguage _steamLanguage;

	private readonly ILoc _loc;

	public bool GameIsAllowedToStart => _steamManager.GameIsAllowedToRun;

	public string Language => _steamLanguage.GetLanguageCode();

	public string ShortUpdateUrl => "https://store.steampowered.com/news/";

	public string FullUpdateUrl => "https://store.steampowered.com/news/app/1062090/view/713408417186711419";

	public string UpdateInfoTextLocKey => "MainMenu.SteamUpdateInfoText";

	public SteamStore(SteamManager steamManager, SteamLanguage steamLanguage, ILoc loc)
	{
		_steamManager = steamManager;
		_steamLanguage = steamLanguage;
		_loc = loc;
	}

	public string GetCompatibilityMessage()
	{
		return Environment.NewLine + Environment.NewLine + _loc.T(SteamCompatibilityMessageLocKey);
	}
}
