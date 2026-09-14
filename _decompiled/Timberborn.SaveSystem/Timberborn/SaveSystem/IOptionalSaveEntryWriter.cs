namespace Timberborn.SaveSystem;

public interface IOptionalSaveEntryWriter : ISaveEntryWriter
{
	bool ShouldWrite { get; }
}
