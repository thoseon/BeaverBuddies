using System.IO;

namespace Timberborn.SaveSystem;

public interface ISaveEntryWriter
{
	string EntryName { get; }

	void WriteToSaveEntryStream(Stream entryStream);
}
