using System.IO;

namespace Timberborn.SaveSystem;

public interface IBackwardCompatibleSaveEntryReader<out T> : ISaveEntryReader<T>
{
	T BackwardCompatibleRead(Stream fileStream);
}
