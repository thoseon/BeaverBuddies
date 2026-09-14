using System.IO;
using Timberborn.ErrorReporting;
using Timberborn.MapThumbnail;
using Timberborn.MapThumbnailOverlaySystem;
using Timberborn.SaveSystem;
using Timberborn.ThumbnailCapturing;

namespace Timberborn.MapThumbnailCapturing;

internal class MapThumbnailSaveEntryWriter : ISaveEntryWriter
{
	private readonly MapThumbnailConfiguration _mapThumbnailConfiguration;

	private readonly ThumbnailSaveEntryWriter _thumbnailSaveEntryWriter;

	private readonly MapThumbnailOverlay _mapThumbnailOverlay;

	public string EntryName => _mapThumbnailConfiguration.Name;

	public MapThumbnailSaveEntryWriter(MapThumbnailConfiguration mapThumbnailConfiguration, ThumbnailSaveEntryWriter thumbnailSaveEntryWriter, MapThumbnailOverlay mapThumbnailOverlay)
	{
		_mapThumbnailConfiguration = mapThumbnailConfiguration;
		_thumbnailSaveEntryWriter = thumbnailSaveEntryWriter;
		_mapThumbnailOverlay = mapThumbnailOverlay;
	}

	public void WriteToSaveEntryStream(Stream entryStream)
	{
		if (!ErrorReporter.ErrorReported)
		{
			_thumbnailSaveEntryWriter.WriteToSaveEntryStream(entryStream, _mapThumbnailConfiguration, _mapThumbnailOverlay.Overlay);
		}
	}
}
