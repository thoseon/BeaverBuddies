using System.Collections.Generic;
using System.IO;
using Timberborn.Modding;
using Timberborn.SteamWorkshopContent;
using Timberborn.Versioning;

namespace Timberborn.SteamWorkshopModDownloading;

public class SteamWorkshopModsProvider : IModsProvider
{
	private readonly SteamWorkshopContentProvider _steamWorkshopContentProvider;

	private readonly ModLoader _modLoader;

	public SteamWorkshopModsProvider(SteamWorkshopContentProvider steamWorkshopContentProvider, ModLoader modLoader)
	{
		_steamWorkshopContentProvider = steamWorkshopContentProvider;
		_modLoader = modLoader;
	}

	public IEnumerable<ModDirectory> GetModDirectories()
	{
		foreach (DirectoryInfo contentDirectory in _steamWorkshopContentProvider.GetContentDirectories())
		{
			if (_modLoader.IsModDirectory(contentDirectory))
			{
				yield return new ModDirectory(contentDirectory, isUserMod: false, "Steam Workshop", GameVersions.CurrentVersion, isSubdirectory: false);
			}
		}
	}
}
