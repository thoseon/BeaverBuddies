using System.IO;
using Timberborn.SaveSystem;
using Timberborn.ThumbnailSystem;
using UnityEngine;

namespace Timberborn.MapThumbnail;

public class MapThumbnailSaveEntryReader : ISaveEntryReader<Texture2D>
{
	private readonly MapThumbnailConfiguration _mapThumbnailConfiguration;

	private readonly ThumbnailSerializer _thumbnailSerializer;

	public string EntryName => _mapThumbnailConfiguration.Name;

	public MapThumbnailSaveEntryReader(MapThumbnailConfiguration mapThumbnailConfiguration, ThumbnailSerializer thumbnailSerializer)
	{
		_mapThumbnailConfiguration = mapThumbnailConfiguration;
		_thumbnailSerializer = thumbnailSerializer;
	}

	public Texture2D ReadFromSaveEntryStream(Stream entryStream)
	{
		return _thumbnailSerializer.ReadFromSaveEntryStream(entryStream, _mapThumbnailConfiguration);
	}
}
