using System;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.MapSystem;
using Timberborn.NewGameConfigurationSystem;
using Timberborn.WorldSerialization;
using UnityEngine;

namespace Timberborn.GameSaveRuntimeSystem;

public class GameLoader
{
	private readonly GameSaveDeserializer _gameSaveDeserializer;

	private readonly MapLoader _mapLoader;

	public bool IsNewGame { get; private set; }

	public SaveReference LoadedSave { get; private set; }

	public GameLoader(GameSaveDeserializer gameSaveDeserializer, MapLoader mapLoader)
	{
		_gameSaveDeserializer = gameSaveDeserializer;
		_mapLoader = mapLoader;
	}

	public SerializedWorld Load(SaveReference saveReference)
	{
		Debug.Log($"Loading saved game {saveReference} at {DateTime.Now:u}");
		LoadedSave = saveReference;
		return _gameSaveDeserializer.Load(saveReference);
	}

	public SerializedWorld LoadNew(NewGameConfiguration newGameConfiguration)
	{
		Debug.Log($"Starting new game at {DateTime.Now:u}:\n{newGameConfiguration}");
		IsNewGame = true;
		return _mapLoader.Load(newGameConfiguration.MapFileReference);
	}
}
