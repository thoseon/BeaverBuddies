using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;

namespace Timberborn.SaveSystem;

public class SaveWriter
{
	private readonly ImmutableArray<ISaveEntryWriter> _saveEntryWriters;

	public SaveWriter(IEnumerable<ISaveEntryWriter> saveFileWriters)
	{
		_saveEntryWriters = saveFileWriters.ToImmutableArray();
	}

	public void WriteToSaveStream(Stream saveStream, bool leaveOpen = false)
	{
		using ZipArchive zipArchive = new ZipArchive(saveStream, ZipArchiveMode.Update, leaveOpen);
		foreach (ISaveEntryWriter saveEntryWriter in _saveEntryWriters)
		{
			if (!(saveEntryWriter is IOptionalSaveEntryWriter { ShouldWrite: false }))
			{
				using Stream entryStream = zipArchive.CreateEntry(saveEntryWriter.EntryName, CompressionLevel.Fastest).Open();
				saveEntryWriter.WriteToSaveEntryStream(entryStream);
			}
		}
	}
}
