using Timberborn.MapRepositorySystem;
using Timberborn.SceneLoading;
using UnityEngine;

namespace Timberborn.MapEditorSceneLoading;

public class MapEditorSceneParameters : ISceneParameters
{
	public bool NewMap { get; }

	public Vector2Int? NewMapSize { get; }

	public MapFileReference? Map { get; }

	public int SceneIndex => 4;

	private MapEditorSceneParameters(bool newMap, Vector2Int? newMapSize, MapFileReference? map)
	{
		NewMap = newMap;
		NewMapSize = newMapSize;
		Map = map;
	}

	public static MapEditorSceneParameters CreateNewMapParameters(Vector2Int mapSize)
	{
		return new MapEditorSceneParameters(newMap: true, mapSize, null);
	}

	public static MapEditorSceneParameters CreateExistingMapParameters(MapFileReference mapFileReference)
	{
		return new MapEditorSceneParameters(newMap: false, null, mapFileReference);
	}
}
