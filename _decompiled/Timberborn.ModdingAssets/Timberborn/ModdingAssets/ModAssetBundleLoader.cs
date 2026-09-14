using System;
using System.Collections.Generic;
using System.IO;
using Timberborn.Common;
using Timberborn.Modding;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ModdingAssets;

public class ModAssetBundleLoader : ILoadableSingleton
{
	private static readonly string AssetBundleDirectory = "AssetBundles";

	private static readonly string WinAssetBundleSuffix = "_win";

	private static readonly string MacAssetBundleSuffix = "_mac";

	private readonly ModRepository _modRepository;

	private readonly List<OrderedAssetBundle> _loadedAssetBundles = new List<OrderedAssetBundle>();

	public ReadOnlyList<OrderedAssetBundle> LoadedAssetBundles => _loadedAssetBundles.AsReadOnlyList();

	public ModAssetBundleLoader(ModRepository modRepository)
	{
		_modRepository = modRepository;
	}

	public void Load()
	{
		FindPreloadedAssetBundles();
		Reload();
	}

	public void Reload()
	{
		foreach (OrderedAssetBundle loadedAssetBundle in _loadedAssetBundles)
		{
			loadedAssetBundle.AssetBundle.Unload(unloadAllLoadedObjects: true);
		}
		_loadedAssetBundles.Clear();
		LoadModAssetBundles();
	}

	private void FindPreloadedAssetBundles()
	{
		foreach (AssetBundle allLoadedAssetBundle in AssetBundle.GetAllLoadedAssetBundles())
		{
			foreach (OrderedFile allModAssetBundleFile in GetAllModAssetBundleFiles())
			{
				if (allModAssetBundleFile.File.Name == allLoadedAssetBundle.name)
				{
					_loadedAssetBundles.Add(new OrderedAssetBundle(allModAssetBundleFile.Order, allLoadedAssetBundle));
					break;
				}
			}
		}
	}

	private void LoadModAssetBundles()
	{
		foreach (OrderedFile allModAssetBundleFile in GetAllModAssetBundleFiles())
		{
			AssetBundle assetBundle = AssetBundle.LoadFromFile(allModAssetBundleFile.File.FullName);
			if ((bool)assetBundle)
			{
				_loadedAssetBundles.Add(new OrderedAssetBundle(allModAssetBundleFile.Order, assetBundle));
				continue;
			}
			throw new InvalidOperationException("Failed to load asset bundle " + allModAssetBundleFile.File.FullName + ", check logs for more information");
		}
	}

	private IEnumerable<OrderedFile> GetAllModAssetBundleFiles()
	{
		foreach (Mod mod in _modRepository.EnabledMods)
		{
			foreach (FileInfo assetBundleFile in GetAssetBundleFiles(mod.ModDirectory.Directory))
			{
				yield return new OrderedFile(_modRepository.Mods.IndexOf(mod), assetBundleFile, mod.DisplayName);
			}
		}
	}

	private static IEnumerable<FileInfo> GetAssetBundleFiles(DirectoryInfo modDirectory)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(modDirectory.FullName, AssetBundleDirectory));
		if (!directoryInfo.Exists)
		{
			yield break;
		}
		FileInfo[] files = directoryInfo.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			if (IsNotManifestFile(fileInfo) && CanBeLoadedOnCurrentPlatform(fileInfo))
			{
				yield return fileInfo;
			}
		}
	}

	private static bool IsNotManifestFile(FileInfo fileInfo)
	{
		return fileInfo.Extension != ".manifest";
	}

	private static bool CanBeLoadedOnCurrentPlatform(FileInfo fileInfo)
	{
		if (!HasSuffix(fileInfo, WinAssetBundleSuffix) || !ApplicationPlatform.IsMacOS())
		{
			if (HasSuffix(fileInfo, MacAssetBundleSuffix))
			{
				return ApplicationPlatform.IsMacOS();
			}
			return true;
		}
		return false;
	}

	private static bool HasSuffix(FileInfo fileInfo, string suffix)
	{
		if (!fileInfo.Name.EndsWith(suffix))
		{
			return Path.GetFileNameWithoutExtension(fileInfo.Name).EndsWith(suffix);
		}
		return true;
	}
}
