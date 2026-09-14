using Timberborn.MainMenuModdingUI;
using Timberborn.SingletonSystem;
using Timberborn.SteamOverlaySystem;
using Timberborn.SteamStoreSystem;

namespace Timberborn.SteamWorkshopModDownloadingUI;

internal class SteamWorkshopModDownloader : ILoadableSingleton
{
	private readonly SteamOverlayOpener _steamOverlayOpener;

	private readonly ModManagerBox _modManagerBox;

	private readonly SteamManager _steamManager;

	public SteamWorkshopModDownloader(SteamOverlayOpener steamOverlayOpener, ModManagerBox modManagerBox, SteamManager steamManager)
	{
		_steamOverlayOpener = steamOverlayOpener;
		_modManagerBox = modManagerBox;
		_steamManager = steamManager;
	}

	public void Load()
	{
		if (_steamManager.Initialized)
		{
			_modManagerBox.SetDownloadAction(ShowDownloadableMods);
		}
	}

	private void ShowDownloadableMods()
	{
		_steamOverlayOpener.OpenWorkshopSearch("Mod");
	}
}
