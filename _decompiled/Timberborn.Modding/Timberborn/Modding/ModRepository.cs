using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Timberborn.SingletonSystem;
using Timberborn.Versioning;
using UnityEngine;

namespace Timberborn.Modding;

public class ModRepository : ILoadableSingleton
{
	private static readonly string DirectoryVersionPrefix = "version-";

	private readonly ModLoader _modLoader;

	private readonly ModSorter _modSorter;

	private readonly ImmutableArray<IModsProvider> _modsProviders;

	public ImmutableArray<Mod> Mods { get; private set; }

	public IEnumerable<Mod> EnabledMods => Mods.Where((Mod mod) => ModdedState.IsModded && mod.IsEnabled);

	public IEnumerable<Mod> UserMods => Mods.Where((Mod mod) => mod.ModDirectory.IsUserMod);

	public ModRepository(ModLoader modLoader, ModSorter modSorter, IEnumerable<IModsProvider> modsProviders)
	{
		_modLoader = modLoader;
		_modSorter = modSorter;
		_modsProviders = modsProviders.ToImmutableArray();
	}

	public void Load()
	{
		Mods = _modSorter.Sort(GetMods()).ToImmutableArray();
		MarkDuplicatedIds();
	}

	public bool ModIsNotEnabled(string modId)
	{
		return EnabledMods.All((Mod mod) => mod.Manifest.Id != modId);
	}

	public bool ModIsOnDifferentVersion(string modId, string version)
	{
		return !EnabledMods.Any((Mod mod) => mod.Manifest.Id == modId && mod.Manifest.Version.Full == version);
	}

	private IEnumerable<Mod> GetMods()
	{
		foreach (IModsProvider modsProvider in _modsProviders)
		{
			foreach (ModDirectory modDirectory in modsProvider.GetModDirectories())
			{
				if (TryGetModDirectory(modDirectory, out var versionedModDirectory) && _modLoader.TryLoadMod(versionedModDirectory, out var mod))
				{
					yield return mod;
				}
			}
		}
	}

	private static bool TryGetModDirectory(ModDirectory modDirectory, out ModDirectory versionedModDirectory)
	{
		try
		{
			versionedModDirectory = GetModDirectory(modDirectory);
			return true;
		}
		catch (Exception arg)
		{
			Debug.LogWarning($"Failed to load mod directory from {modDirectory.Path}: {arg}");
			versionedModDirectory = default(ModDirectory);
			return false;
		}
	}

	private static ModDirectory GetModDirectory(ModDirectory modDirectory)
	{
		DirectoryInfo[] directories = modDirectory.Directory.GetDirectories(DirectoryVersionPrefix + "*");
		if (directories.Length == 0)
		{
			return modDirectory;
		}
		return GetDirectoryForCurrentVersion(GetModSubdirectories(directories, modDirectory), modDirectory);
	}

	private static IEnumerable<ModDirectory> GetModSubdirectories(IEnumerable<DirectoryInfo> directoryInfos, ModDirectory original)
	{
		foreach (DirectoryInfo directoryInfo in directoryInfos)
		{
			string name = directoryInfo.Name;
			int length = DirectoryVersionPrefix.Length;
			string text = name.Substring(length, name.Length - length);
			Timberborn.Versioning.Version gameVersion = (string.IsNullOrEmpty(text) ? GameVersions.CurrentVersion : Timberborn.Versioning.Version.Create(text));
			yield return new ModDirectory(directoryInfo, original.IsUserMod, original.DisplaySource, gameVersion, isSubdirectory: true);
		}
	}

	private static ModDirectory GetDirectoryForCurrentVersion(IEnumerable<ModDirectory> subdirectories, ModDirectory original)
	{
		Timberborn.Versioning.Version currentVersion = GameVersions.CurrentVersion;
		ModDirectory? modDirectory = null;
		foreach (ModDirectory subdirectory in subdirectories)
		{
			if ((currentVersion.IsEqualOrHigherThan(subdirectory.GameVersion) || currentVersion.IsDevelopmentVersion) && (!modDirectory.HasValue || subdirectory.GameVersion.IsEqualOrHigherThan(modDirectory.Value.GameVersion)))
			{
				modDirectory = subdirectory;
			}
		}
		if (!modDirectory.HasValue)
		{
			throw new InvalidOperationException($"No matching version directory found for version {currentVersion} in {original.Path}");
		}
		return modDirectory.Value;
	}

	private void MarkDuplicatedIds()
	{
		foreach (Mod item in (from mod in Mods
			group mod by new
			{
				mod.Manifest.Id,
				mod.ModDirectory.IsUserMod
			} into @group
			where @group.Count() > 1
			select @group).SelectMany(group => group))
		{
			item.MarkIdAsDuplicated();
		}
	}
}
