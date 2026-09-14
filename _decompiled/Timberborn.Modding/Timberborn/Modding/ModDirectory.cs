using System.IO;
using Timberborn.Versioning;

namespace Timberborn.Modding;

public readonly struct ModDirectory
{
	private readonly bool _isSubdirectory;

	public DirectoryInfo Directory { get; }

	public bool IsUserMod { get; }

	public string DisplaySource { get; }

	public Version GameVersion { get; }

	public string Path => Directory.FullName;

	public string OriginPath
	{
		get
		{
			if (!_isSubdirectory)
			{
				return Path;
			}
			return Directory.Parent.FullName;
		}
	}

	public string OriginName
	{
		get
		{
			if (!_isSubdirectory)
			{
				return Directory.Name;
			}
			return Directory.Parent.Name;
		}
	}

	public ModDirectory(DirectoryInfo directory, bool isUserMod, string displaySource, Version gameVersion, bool isSubdirectory)
	{
		Directory = directory;
		IsUserMod = isUserMod;
		DisplaySource = displaySource;
		GameVersion = gameVersion;
		_isSubdirectory = isSubdirectory;
	}
}
