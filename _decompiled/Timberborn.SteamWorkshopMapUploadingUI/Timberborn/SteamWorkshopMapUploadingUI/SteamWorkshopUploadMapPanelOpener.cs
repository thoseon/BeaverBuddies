using System;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.MapEditorPersistenceUI;
using Timberborn.MapEditorUI;
using Timberborn.SingletonSystem;
using Timberborn.SteamStoreSystem;
using Timberborn.SteamWorkshopUI;

namespace Timberborn.SteamWorkshopMapUploadingUI;

internal class SteamWorkshopUploadMapPanelOpener : ILoadableSingleton
{
	private static readonly string UploadLocKey = "MapEditor.UploadMap";

	private static readonly string MapWillBeSavedLocKey = "MapEditor.MapWillBeSaved";

	private readonly SteamWorkshopUploadPanel _steamWorkshopUploadPanel;

	private readonly SteamWorkshopUploadableMapFactory _steamWorkshopUploadableMapFactory;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly MapPersistenceController _mapPersistenceController;

	private readonly MapSaverLoader _mapSaverLoader;

	private readonly ILoc _loc;

	private readonly FilePanel _filePanel;

	private readonly SteamManager _steamManager;

	public SteamWorkshopUploadMapPanelOpener(SteamWorkshopUploadPanel steamWorkshopUploadPanel, SteamWorkshopUploadableMapFactory steamWorkshopUploadableMapFactory, DialogBoxShower dialogBoxShower, MapPersistenceController mapPersistenceController, MapSaverLoader mapSaverLoader, ILoc loc, FilePanel filePanel, SteamManager steamManager)
	{
		_steamWorkshopUploadPanel = steamWorkshopUploadPanel;
		_steamWorkshopUploadableMapFactory = steamWorkshopUploadableMapFactory;
		_dialogBoxShower = dialogBoxShower;
		_mapPersistenceController = mapPersistenceController;
		_mapSaverLoader = mapSaverLoader;
		_loc = loc;
		_filePanel = filePanel;
		_steamManager = steamManager;
	}

	public void Load()
	{
		if (_steamManager.Initialized)
		{
			_filePanel.AddMapFileButton(Open, UploadLocKey);
		}
	}

	private void Open()
	{
		_dialogBoxShower.Create().SetLocalizedMessage(MapWillBeSavedLocKey).SetConfirmButton(delegate
		{
			_mapSaverLoader.Save(OpenUploadPanel);
		}, _loc.T(CommonLocKeys.OKKey))
			.SetDefaultCancelButton(_loc.T(CommonLocKeys.CancelKey))
			.Show();
	}

	private void OpenUploadPanel()
	{
		if (_mapPersistenceController.TryGetCurrentMap(out var mapFileReference))
		{
			SteamWorkshopUploadableMap steamWorkshopUploadable = _steamWorkshopUploadableMapFactory.Create(mapFileReference);
			_steamWorkshopUploadPanel.Open(steamWorkshopUploadable);
			return;
		}
		throw new InvalidOperationException("Tried to upload unsaved map to Steam");
	}
}
