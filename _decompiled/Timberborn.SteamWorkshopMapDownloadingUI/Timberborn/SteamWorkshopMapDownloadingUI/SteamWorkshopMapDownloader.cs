using Timberborn.MapRepositorySystemUI;
using Timberborn.SingletonSystem;
using Timberborn.SteamOverlaySystem;
using Timberborn.SteamStoreSystem;

namespace Timberborn.SteamWorkshopMapDownloadingUI;

internal class SteamWorkshopMapDownloader : ILoadableSingleton
{
	private readonly SteamOverlayOpener _steamOverlayOpener;

	private readonly MapDownloader _mapDownloader;

	private readonly SteamManager _steamManager;

	public SteamWorkshopMapDownloader(SteamOverlayOpener steamOverlayOpener, MapDownloader mapDownloader, SteamManager steamManager)
	{
		_steamOverlayOpener = steamOverlayOpener;
		_mapDownloader = mapDownloader;
		_steamManager = steamManager;
	}

	public void Load()
	{
		if (_steamManager.Initialized)
		{
			_mapDownloader.SetDownloadAction(ShowDownloadableMaps);
		}
	}

	private void ShowDownloadableMaps()
	{
		_steamOverlayOpener.OpenWorkshopSearch("Map");
	}
}
