using System.IO;
using Timberborn.MapMetadataSystem;
using Timberborn.SaveSystem;

namespace Timberborn.MapMetadataSystemUI;

internal class MapMetadataSaveEntryWriter : ISaveEntryWriter
{
	private readonly MapMetadataSerializer _mapMetadataSerializer;

	public MapMetadata CurrentMapMetadata { get; private set; }

	public string EntryName => _mapMetadataSerializer.EntryName;

	public MapMetadataSaveEntryWriter(MapMetadataSerializer mapMetadataSerializer)
	{
		_mapMetadataSerializer = mapMetadataSerializer;
	}

	public void SetCurrentMapMetadata(MapMetadata mapMetadata)
	{
		CurrentMapMetadata = mapMetadata;
	}

	public void WriteToSaveEntryStream(Stream entryStream)
	{
		_mapMetadataSerializer.WriteToSaveEntryStream(entryStream, CurrentMapMetadata);
	}
}
