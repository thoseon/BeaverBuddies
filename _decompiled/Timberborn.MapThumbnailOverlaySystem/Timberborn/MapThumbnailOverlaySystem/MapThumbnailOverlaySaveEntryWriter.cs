using System.IO;
using Timberborn.SaveSystem;

namespace Timberborn.MapThumbnailOverlaySystem;

internal class MapThumbnailOverlaySaveEntryWriter : IOptionalSaveEntryWriter, ISaveEntryWriter
{
	private readonly MapThumbnailOverlay _mapThumbnailOverlay;

	private readonly MapThumbnailOverlaySerializer _mapThumbnailOverlaySerializer;

	public string EntryName => _mapThumbnailOverlaySerializer.EntryName;

	public bool ShouldWrite => _mapThumbnailOverlay.Overlay;

	public MapThumbnailOverlaySaveEntryWriter(MapThumbnailOverlay mapThumbnailOverlay, MapThumbnailOverlaySerializer mapThumbnailOverlaySerializer)
	{
		_mapThumbnailOverlay = mapThumbnailOverlay;
		_mapThumbnailOverlaySerializer = mapThumbnailOverlaySerializer;
	}

	public void WriteToSaveEntryStream(Stream entryStream)
	{
		_mapThumbnailOverlaySerializer.WriteToSaveEntryStream(entryStream, _mapThumbnailOverlay.Overlay);
	}
}
