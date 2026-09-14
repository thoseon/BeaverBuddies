using Timberborn.ApplicationLifetime;
using Timberborn.GameSaveRuntimeSystem;
using Timberborn.GameSceneLoading;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;
using Timberborn.WorldSerialization;
using UnityEngine;

namespace Timberborn.GameScene;

public class GameSceneSerializedWorldSupplier : ISerializedWorldSupplier, ILoadableSingleton, INonSingletonPostLoader
{
	private readonly GameLoader _gameLoader;

	private readonly ISceneLoader _sceneLoader;

	private SerializedWorld _serializedWorld;

	public GameSceneSerializedWorldSupplier(GameLoader gameLoader, ISceneLoader sceneLoader)
	{
		_gameLoader = gameLoader;
		_sceneLoader = sceneLoader;
	}

	public void Load()
	{
		GameSceneParameters sceneParameters = _sceneLoader.GetSceneParameters<GameSceneParameters>();
		_serializedWorld = LoadGame(sceneParameters);
	}

	public void PostLoadNonSingletons()
	{
		_serializedWorld = null;
	}

	public SerializedWorld Get()
	{
		return _serializedWorld;
	}

	private SerializedWorld LoadGame(GameSceneParameters gameSceneParameters)
	{
		try
		{
			return gameSceneParameters.NewGame ? _gameLoader.LoadNew(gameSceneParameters.NewGameConfiguration) : _gameLoader.Load(gameSceneParameters.SaveReference);
		}
		catch
		{
			if (Application.isEditor)
			{
				GameQuitter.Quit();
			}
			throw;
		}
	}
}
