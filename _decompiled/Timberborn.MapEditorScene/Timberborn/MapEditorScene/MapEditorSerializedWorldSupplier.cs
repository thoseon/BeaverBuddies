using System;
using Timberborn.ApplicationLifetime;
using Timberborn.MapEditorPersistence;
using Timberborn.MapEditorSceneLoading;
using Timberborn.MapRepositorySystem;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;
using Timberborn.WorldSerialization;
using UnityEngine;

namespace Timberborn.MapEditorScene;

public class MapEditorSerializedWorldSupplier : ISerializedWorldSupplier, ILoadableSingleton, INonSingletonPostLoader
{
	private readonly MapEditorMapLoader _mapEditorMapLoader;

	private readonly ISceneLoader _sceneLoader;

	private SerializedWorld _serializedWorld;

	public MapEditorSerializedWorldSupplier(MapEditorMapLoader mapEditorMapLoader, ISceneLoader sceneLoader)
	{
		_mapEditorMapLoader = mapEditorMapLoader;
		_sceneLoader = sceneLoader;
	}

	public void Load()
	{
		MapEditorSceneParameters sceneParameters = _sceneLoader.GetSceneParameters<MapEditorSceneParameters>();
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

	private SerializedWorld LoadGame(MapEditorSceneParameters mapEditorSceneParameters)
	{
		try
		{
			if (mapEditorSceneParameters.NewMap)
			{
				Vector2Int size = mapEditorSceneParameters.NewMapSize ?? throw new InvalidOperationException("Missing map size");
				return _mapEditorMapLoader.LoadNew(size);
			}
			MapFileReference mapFileReference = mapEditorSceneParameters.Map ?? throw new InvalidOperationException("Missing map parameter");
			return _mapEditorMapLoader.Load(mapFileReference);
		}
		catch (Exception)
		{
			if (Application.isEditor)
			{
				GameQuitter.Quit();
			}
			throw;
		}
	}
}
