using Timberborn.PlayerDataSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Analytics;

public class AnalyticsConsent : ILoadableSingleton
{
	private static readonly string ConsentKey = "AnalyticsConsent_IsConsentGiven";

	private readonly IPlayerDataService _playerDataService;

	public bool IsConsentGiven { get; private set; }

	public bool WasConsentAsked => _playerDataService.HasKey(ConsentKey);

	public AnalyticsConsent(IPlayerDataService playerDataService)
	{
		_playerDataService = playerDataService;
	}

	public void Load()
	{
		IsConsentGiven = _playerDataService.HasKey(ConsentKey) && _playerDataService.GetBool(ConsentKey, defaultValue: false);
	}

	public void GiveConsent()
	{
		IsConsentGiven = true;
		_playerDataService.SetBool(ConsentKey, value: true);
	}

	public void RemoveConsent()
	{
		IsConsentGiven = false;
		_playerDataService.SetBool(ConsentKey, value: false);
	}
}
