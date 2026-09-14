using UnityEngine;

namespace Timberborn.WebNavigation;

public class UrlOpener
{
	public static readonly string BugInfoUrl = "https://mechanistry.com/bug";

	public static readonly string HowToRemoveModsUrl = "https://mechanistry.com/how-to-remove-mods";

	public static readonly string DiscordUrl = "https://discord.gg/timberborn";

	public static readonly string MerchandiseUrl = "https://merch.timberborn.com/";

	private static readonly string FeatureUpvoteUrl = "https://timberborn.featureupvote.com/";

	private static readonly string PrivacyPolicyUrl = "https://mechanistry.com/privacy";

	private static readonly string AnalyticsPrivacyPolicyUrl = "https://mechanistry.com/privacy";

	private static readonly string ModdingDocumentationUrl = "https://github.com/mechanistry/timberborn-modding/wiki";

	public void OpenDiscord()
	{
		OpenUrl(DiscordUrl);
	}

	public void OpenMerchandise()
	{
		OpenUrl(MerchandiseUrl);
	}

	public void OpenBugInfo()
	{
		OpenUrl(BugInfoUrl);
	}

	public void OpenHowToRemoveMods()
	{
		OpenUrl(HowToRemoveModsUrl);
	}

	public void OpenPrivacyPolicy()
	{
		OpenUrl(PrivacyPolicyUrl);
	}

	public void OpenAnalyticsPrivacyPolicy()
	{
		OpenUrl(AnalyticsPrivacyPolicyUrl);
	}

	public void OpenFeatureUpvote()
	{
		OpenUrl(FeatureUpvoteUrl);
	}

	public void OpenModdingDocumentation()
	{
		OpenUrl(ModdingDocumentationUrl);
	}

	public void OpenUrl(string url)
	{
		Application.OpenURL(url);
	}
}
