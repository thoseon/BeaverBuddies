using System;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.MapEditorPersistence;
using Timberborn.MapRepositorySystem;
using Timberborn.MapRepositorySystemUI;
using Timberborn.MapSystem;
using Timberborn.QuickNotificationSystem;
using Timberborn.SingletonSystem;
using Timberborn.Versioning;
using UnityEngine;

namespace Timberborn.MapEditorPersistenceUI;

public class MapPersistenceController
{
	private static readonly string MapExistsLocKey = "MapEditor.SaveMap.MapExists";

	private static readonly string SavedAsLocKey = "MapEditor.SaveMap.SavedAs";

	private static readonly string ErrorLocKey = "Saving.Error";

	private static readonly Timberborn.Versioning.Version NewMapVersion = Timberborn.Versioning.Version.Create("0");

	private readonly MapEditorMapLoader _mapEditorMapLoader;

	private readonly MapSaver _mapSaver;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly QuickNotificationService _quickNotificationService;

	private readonly ILoc _loc;

	private readonly MapVersionCompatibilityService _mapVersionCompatibilityService;

	private readonly EventBus _eventBus;

	public Timberborn.Versioning.Version CurrentMapVersion
	{
		get
		{
			if (!TryGetCurrentMap(out var mapFileReference))
			{
				return NewMapVersion;
			}
			return _mapVersionCompatibilityService.GetMapVersionNumber(mapFileReference);
		}
	}

	public bool IsCurrentMapCompatible
	{
		get
		{
			if (TryGetCurrentMap(out var mapFileReference))
			{
				return _mapVersionCompatibilityService.IsMapFullyCompatible(mapFileReference);
			}
			return true;
		}
	}

	public MapPersistenceController(MapEditorMapLoader mapEditorMapLoader, MapSaver mapSaver, DialogBoxShower dialogBoxShower, QuickNotificationService quickNotificationService, ILoc loc, MapVersionCompatibilityService mapVersionCompatibilityService, EventBus eventBus)
	{
		_mapEditorMapLoader = mapEditorMapLoader;
		_mapSaver = mapSaver;
		_dialogBoxShower = dialogBoxShower;
		_quickNotificationService = quickNotificationService;
		_loc = loc;
		_mapVersionCompatibilityService = mapVersionCompatibilityService;
		_eventBus = eventBus;
	}

	public void SaveAs(string mapName, Action successAction)
	{
		try
		{
			if (_mapSaver.MapExists(mapName))
			{
				_dialogBoxShower.Create().SetMessage(_loc.T(MapExistsLocKey, mapName)).SetConfirmButton(delegate
				{
					ForceSaveAs(mapName, successAction, notify: true);
				}, _loc.T(CommonLocKeys.OverwriteKey))
					.SetDefaultCancelButton(_loc.T(CommonLocKeys.CancelKey))
					.Show();
			}
			else
			{
				ForceSaveAs(mapName, successAction, notify: true);
			}
		}
		catch (MapSaverException ex)
		{
			Debug.LogError($"Error occured while saving: {ex.InnerException}");
			_dialogBoxShower.Create().SetLocalizedMessage(ErrorLocKey).Show();
		}
	}

	public bool TrySaveCurrent(Action successAction)
	{
		return TrySaveCurrentInternal(notify: true, successAction);
	}

	public void SaveCurrentSilently()
	{
		if (!TrySaveCurrentInternal(notify: false, null))
		{
			throw new InvalidOperationException("No map to save");
		}
	}

	public bool TryGetCurrentMap(out MapFileReference mapFileReference)
	{
		if (_mapSaver.LastSavedMap.HasValue)
		{
			mapFileReference = _mapSaver.LastSavedMap.Value;
			return true;
		}
		if (_mapEditorMapLoader.LoadedMap.HasValue)
		{
			mapFileReference = _mapEditorMapLoader.LoadedMap.Value;
			return mapFileReference.UserFolder;
		}
		mapFileReference = default(MapFileReference);
		return false;
	}

	private bool TrySaveCurrentInternal(bool notify, Action successAction)
	{
		if (TryGetCurrentMap(out var mapFileReference))
		{
			ForceSaveAs(mapFileReference.Name, successAction, notify);
			return true;
		}
		return false;
	}

	private void ForceSaveAs(string mapName, Action successAction, bool notify)
	{
		try
		{
			_mapSaver.Save(MapFileReference.FromUserFolder(mapName));
			successAction?.Invoke();
			if (notify)
			{
				_quickNotificationService.SendNotification(_loc.T(SavedAsLocKey, mapName));
			}
			_eventBus.Post(new MapSavedEvent());
		}
		catch (MapSaverException ex)
		{
			Debug.LogError($"Error occured while saving: {ex.InnerException}");
			_dialogBoxShower.Create().SetLocalizedMessage(ErrorLocKey).Show();
		}
	}
}
