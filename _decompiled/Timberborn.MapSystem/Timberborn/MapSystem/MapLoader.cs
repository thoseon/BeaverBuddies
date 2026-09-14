using System;
using System.Collections.Generic;
using Timberborn.MapRepositorySystem;
using Timberborn.MapStateSystem;
using Timberborn.WorldPersistence;
using Timberborn.WorldSerialization;
using UnityEngine;

namespace Timberborn.MapSystem;

public class MapLoader
{
	private readonly MapDeserializer _mapDeserializer;

	private readonly SerializedWorldFactory _serializedWorldFactory;

	private bool _loaded;

	public MapLoader(MapDeserializer mapDeserializer, SerializedWorldFactory serializedWorldFactory)
	{
		_mapDeserializer = mapDeserializer;
		_serializedWorldFactory = serializedWorldFactory;
	}

	public SerializedWorld Load(MapFileReference mapFileReference)
	{
		ValidateDuplicateLoad();
		return _mapDeserializer.Load(mapFileReference);
	}

	public SerializedWorld LoadNewMap(Vector2Int size)
	{
		ValidateDuplicateLoad();
		List<ISaveableSingleton> singletons = new List<ISaveableSingleton> { MapSize.NewMap(size) };
		return _serializedWorldFactory.Create(singletons);
	}

	private void ValidateDuplicateLoad()
	{
		if (_loaded)
		{
			throw new InvalidOperationException("Cannot load a map twice in the same scene.");
		}
		_loaded = true;
	}
}
