using System.IO;
using Timberborn.SaveSystem;
using Timberborn.ThumbnailSystem;
using UnityEngine;

namespace Timberborn.SaveThumbnail;

public class SaveThumbnailSaveEntryReader : ISaveEntryReader<Texture2D>
{
	private readonly SaveThumbnailConfiguration _saveThumbnailConfiguration;

	private readonly ThumbnailSerializer _thumbnailSerializer;

	public string EntryName => _saveThumbnailConfiguration.Name;

	public SaveThumbnailSaveEntryReader(SaveThumbnailConfiguration saveThumbnailConfiguration, ThumbnailSerializer thumbnailSerializer)
	{
		_saveThumbnailConfiguration = saveThumbnailConfiguration;
		_thumbnailSerializer = thumbnailSerializer;
	}

	public Texture2D ReadFromSaveEntryStream(Stream entryStream)
	{
		return _thumbnailSerializer.ReadFromSaveEntryStream(entryStream, _saveThumbnailConfiguration);
	}
}
