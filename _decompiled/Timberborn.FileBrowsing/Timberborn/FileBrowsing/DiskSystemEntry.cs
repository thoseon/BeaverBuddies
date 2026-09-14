using System.IO;

namespace Timberborn.FileBrowsing;

public readonly struct DiskSystemEntry
{
	public string Parent { get; }

	public string Name { get; }

	public string Path { get; }

	public bool IsDirectory { get; }

	public bool Exists { get; }

	private DiskSystemEntry(string parent, string name, string path, bool isDirectory, bool exists)
	{
		Parent = parent;
		Name = name;
		Path = path;
		IsDirectory = isDirectory;
		Exists = exists;
	}

	public static DiskSystemEntry Create(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return new DiskSystemEntry(null, "", "", isDirectory: true, exists: true);
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		if (directoryInfo.Exists)
		{
			return new DiskSystemEntry(directoryInfo.Parent?.FullName, directoryInfo.Name, directoryInfo.FullName, isDirectory: true, exists: true);
		}
		FileInfo fileInfo = new FileInfo(path);
		if (fileInfo.Exists)
		{
			return new DiskSystemEntry(fileInfo.Directory?.FullName, fileInfo.Name, fileInfo.FullName, isDirectory: false, exists: true);
		}
		return new DiskSystemEntry(null, "", "", isDirectory: false, exists: false);
	}
}
