using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using UnityEngine;

namespace Timberborn.FileBrowsing;

public class FileFilter
{
	private readonly ImmutableArray<string> _extensions;

	public Sprite Icon { get; }

	public FileFilter(Sprite icon, IEnumerable<string> extensions)
	{
		Icon = icon;
		_extensions = extensions.ToImmutableArray();
	}

	public bool IsValidFile(FileSystemInfo fileSystemInfo)
	{
		return _extensions.Contains(fileSystemInfo.Extension);
	}
}
