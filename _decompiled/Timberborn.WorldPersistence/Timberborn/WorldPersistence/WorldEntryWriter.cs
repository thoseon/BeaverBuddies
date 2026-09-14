using System.IO;
using Timberborn.SaveSystem;
using Timberborn.WorldSerialization;

namespace Timberborn.WorldPersistence;

internal class WorldEntryWriter : ISaveEntryWriter
{
	private readonly SerializedWorldFactory _serializedWorldFactory;

	private readonly WorldSerializer _worldSerializer;

	public string EntryName => _worldSerializer.EntryName;

	public WorldEntryWriter(SerializedWorldFactory serializedWorldFactory, WorldSerializer worldSerializer)
	{
		_serializedWorldFactory = serializedWorldFactory;
		_worldSerializer = worldSerializer;
	}

	public void WriteToSaveEntryStream(Stream entryStream)
	{
		SerializedWorld serializedWorld = _serializedWorldFactory.Create();
		_worldSerializer.WriteToSaveEntryStream(entryStream, serializedWorld);
	}
}
