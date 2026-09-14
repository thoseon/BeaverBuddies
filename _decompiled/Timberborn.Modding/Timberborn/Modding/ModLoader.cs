using System.IO;
using System.Linq;
using UnityEngine;

namespace Timberborn.Modding;

public class ModLoader
{
	private readonly ManifestLoader _manifestLoader;

	public ModLoader(ManifestLoader manifestLoader)
	{
		_manifestLoader = manifestLoader;
	}

	public bool TryLoadMod(ModDirectory modDirectory, out Mod mod)
	{
		if (_manifestLoader.TryLoadManifest(modDirectory.Path, out var manifest))
		{
			if (ValidateId(manifest, modDirectory))
			{
				bool isEnabled = ModPlayerPrefsHelper.IsModEnabled(modDirectory, manifest);
				mod = new Mod(modDirectory, manifest, isEnabled);
				return true;
			}
		}
		else
		{
			Debug.LogWarning("No manifest file found in " + modDirectory.Path);
		}
		mod = null;
		return false;
	}

	public bool IsModDirectory(DirectoryInfo directory)
	{
		return directory.EnumerateFiles(ManifestLoader.ManifestFileName, SearchOption.AllDirectories).Any();
	}

	private static bool ValidateId(ModManifest manifest, ModDirectory modDirectory)
	{
		if (string.IsNullOrWhiteSpace(manifest.Id))
		{
			Debug.LogWarning("Mod id from directory \"" + modDirectory.Path + "\" is empty");
			return false;
		}
		return true;
	}
}
