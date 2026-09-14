using Steamworks;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.SteamStoreSystem;

namespace Timberborn.SteamOSUI;

internal class SteamDeckUIScaleSetter : ILoadableSingleton
{
	private static readonly float IncreasedUIScaleFactor = 1.3f;

	private readonly SteamManager _steamManager;

	private readonly UISettings _uiSettings;

	private readonly UIScaler _uiScaler;

	public SteamDeckUIScaleSetter(SteamManager steamManager, UISettings uiSettings, UIScaler uiScaler)
	{
		_steamManager = steamManager;
		_uiSettings = uiSettings;
		_uiScaler = uiScaler;
	}

	public void Load()
	{
		if (ShouldIncreaseUIScale())
		{
			_uiSettings.UIScaleFactor = _uiScaler.ClampScaleFactor(IncreasedUIScaleFactor);
		}
	}

	private bool ShouldIncreaseUIScale()
	{
		if (_steamManager.Initialized && !_uiSettings.HasStoredUIScaleFactor)
		{
			return SteamUtils.IsSteamRunningOnSteamDeck();
		}
		return false;
	}
}
