using Steamworks;
using Timberborn.SteamStoreSystem;

namespace Timberborn.SteamOverlaySystem;

public class SteamOverlayOpener
{
	private static readonly string WorkshopItemUrl = "https://steamcommunity.com/sharedfiles/filedetails/?id={0}";

	private static readonly string LegalAgreementUrl = "https://steamcommunity.com/sharedfiles/workshoplegalagreement";

	private static readonly string WorkshopSearchUrl = "https://steamcommunity.com/workshop/browse/?appid={0}&requiredtags[]={1}";

	public void OpenLegalAgreement()
	{
		OpenSteamPage(LegalAgreementUrl);
	}

	public void OpenWorkshopItem(ulong workshopItemId)
	{
		OpenSteamPage(string.Format(WorkshopItemUrl, workshopItemId));
	}

	public void OpenWorkshopSearch(string tag)
	{
		OpenSteamPage(string.Format(WorkshopSearchUrl, SteamAppId.AppId, tag));
	}

	private static void OpenSteamPage(string page)
	{
		SteamFriends.ActivateGameOverlayToWebPage(page);
	}
}
