using System;
using System.IO;
using Timberborn.CommandLine;
using Timberborn.ExperimentalModeSystem;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.GameSceneLoading;
using Timberborn.MapEditorSceneLoading;
using Timberborn.MapRepositorySystem;
using Timberborn.PlatformUtilities;
using UnityEngine;

namespace Timberborn.MainMenuScene;

internal class AutoStarter
{
	private static readonly string SettlementNameCommandLineArgumentKey = "settlementName";

	private static readonly string SaveNameCommandLineArgumentKey = "saveName";

	private readonly ExperimentalMode _experimentalMode;

	private readonly GameSceneLoader _gameSceneLoader;

	private readonly ICommandLineArguments _commandLineArguments;

	private readonly MapEditorSceneLoader _mapEditorSceneLoader;

	private string SettlementName => _commandLineArguments.GetString(SettlementNameCommandLineArgumentKey);

	private string SaveName => _commandLineArguments.GetString(SaveNameCommandLineArgumentKey);

	private bool AutoStartingInEditor => false;

	private bool AutoStartingInStandalone
	{
		get
		{
			if (!Application.isEditor)
			{
				return _commandLineArguments.Has(SaveNameCommandLineArgumentKey);
			}
			return false;
		}
	}

	public AutoStarter(ExperimentalMode experimentalMode, GameSceneLoader gameSceneLoader, ICommandLineArguments commandLineArguments, MapEditorSceneLoader mapEditorSceneLoader)
	{
		_commandLineArguments = commandLineArguments;
		_experimentalMode = experimentalMode;
		_gameSceneLoader = gameSceneLoader;
		_mapEditorSceneLoader = mapEditorSceneLoader;
	}

	public void CheckAutoStarting(Action nextAction)
	{
		if (AutoStartingInEditor)
		{
			StartInEditorMode();
		}
		else if (AutoStartingInStandalone)
		{
			if (_experimentalMode.IsExperimental)
			{
				LoadSave(new SaveReference(SaveName, new SettlementReference(SettlementName, Path.Combine(UserDataFolder.Folder, "ExperimentalSaves"))));
			}
			else
			{
				LoadSave(new SaveReference(SaveName, new SettlementReference(SettlementName, Path.Combine(UserDataFolder.Folder, "Saves"))));
			}
		}
		else
		{
			nextAction();
		}
	}

	private void StartInEditorMode()
	{
	}

	private void LoadMostRecentSave()
	{
		_gameSceneLoader.StartMostRecentSaveInstantly();
	}

	private void LoadSave(SaveReference saveReference)
	{
		_gameSceneLoader.StartSaveGameInstantly(saveReference);
	}

	private void StartNewMap()
	{
		_mapEditorSceneLoader.StartNewMapInstantly(new Vector2Int(128, 128));
	}

	private void EditMap(MapFileReference mapFileReference)
	{
		_mapEditorSceneLoader.LoadMapInstantly(mapFileReference);
	}
}
