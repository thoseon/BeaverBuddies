using System.IO;

namespace Timberborn.ModdingAssets;

internal readonly struct OrderedFile
{
	public int Order { get; }

	public FileInfo File { get; }

	public string Source { get; }

	public OrderedFile(int order, FileInfo file, string source)
	{
		Order = order;
		File = file;
		Source = source;
	}
}
