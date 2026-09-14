using System.IO;
using Timberborn.SaveSystem;
using Timberborn.Versioning;
using Timberborn.WorldSerialization;

namespace Timberborn.VersioningSerialization;

public class VersionSerializer : IBackwardCompatibleSaveEntryReader<Version>, ISaveEntryReader<Version>, ISaveEntryWriter
{
	private readonly SaveReader _saveReader;

	private readonly WorldSerializer _worldSerializer;

	public string EntryName => "version.txt";

	public VersionSerializer(SaveReader saveReader, WorldSerializer worldSerializer)
	{
		_saveReader = saveReader;
		_worldSerializer = worldSerializer;
	}

	public void WriteToSaveEntryStream(Stream entryStream)
	{
		using StreamWriter streamWriter = new StreamWriter(entryStream);
		streamWriter.WriteLine(GameVersions.CurrentVersion.Full);
	}

	public Version ReadFromSaveEntryStream(Stream entryStream)
	{
		using StreamReader streamReader = new StreamReader(entryStream);
		return Version.Create(streamReader.ReadLine());
	}

	public Version BackwardCompatibleRead(Stream fileStream)
	{
		return _saveReader.ReadFromSaveStream(fileStream, _worldSerializer)?.Version ?? Version.Create("0");
	}
}
