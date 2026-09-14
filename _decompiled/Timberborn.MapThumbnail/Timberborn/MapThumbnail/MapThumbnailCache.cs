using Timberborn.MapRepositorySystem;
using Timberborn.SingletonSystem;
using Timberborn.ThumbnailSystem;
using UnityEngine;

namespace Timberborn.MapThumbnail;

public class MapThumbnailCache : ILoadableSingleton, IUnloadableSingleton
{
	private readonly MapDeserializer _mapDeserializer;

	private readonly MapThumbnailSaveEntryReader _mapThumbnailSaveEntryReader;

	private ThumbnailCache<MapFileReference> _thumbnailCache;

	public MapThumbnailCache(MapDeserializer mapDeserializer, MapThumbnailSaveEntryReader mapThumbnailSaveEntryReader)
	{
		_mapDeserializer = mapDeserializer;
		_mapThumbnailSaveEntryReader = mapThumbnailSaveEntryReader;
	}

	public void Load()
	{
		_thumbnailCache = new ThumbnailCache<MapFileReference>((MapFileReference reference) => _mapDeserializer.ReadFromMapFile(reference, _mapThumbnailSaveEntryReader));
	}

	public void Unload()
	{
		Clear();
	}

	public void Clear()
	{
		_thumbnailCache.Clear();
	}

	public Texture2D GetThumbnail(MapFileReference mapFileReference)
	{
		return _thumbnailCache.GetThumbnail(mapFileReference);
	}
}
