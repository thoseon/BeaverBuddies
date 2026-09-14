using System.IO;
using Timberborn.SerializationSystem;

namespace Timberborn.ModdingAssets;

internal interface IModFileConverter<T>
{
	bool CanConvert(FileInfo fileInfo);

	bool TryConvert(OrderedFile orderedFile, string path, SerializedObject metadata, out T asset);

	void Reset();
}
