using System;
using Timberborn.MapRepositorySystem;
using Timberborn.MapSystem;
using Timberborn.WorldSerialization;
using UnityEngine;

namespace Timberborn.MapEditorPersistence;

public class MapEditorMapLoader
{
	private readonly MapLoader _mapLoader;

	public MapFileReference? LoadedMap { get; private set; }

	public MapEditorMapLoader(MapLoader mapLoader)
	{
		_mapLoader = mapLoader;
	}

	public SerializedWorld Load(MapFileReference mapFileReference)
	{
		Debug.Log($"Loading map from {mapFileReference} at {DateTime.Now:u}");
		LoadedMap = mapFileReference;
		return _mapLoader.Load(mapFileReference);
	}

	public SerializedWorld LoadNew(Vector2Int size)
	{
		Debug.Log($"Creating new {size.x}x{size.y} map at {DateTime.Now:u}");
		return _mapLoader.LoadNewMap(size);
	}
}
