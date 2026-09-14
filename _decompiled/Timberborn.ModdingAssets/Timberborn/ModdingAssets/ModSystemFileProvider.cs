using System.Collections.Generic;
using System.IO;
using Timberborn.AssetSystem;
using Timberborn.Modding;
using Timberborn.SerializationSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ModdingAssets;

internal class ModSystemFileProvider<T> : ILoadableSingleton, IAssetProvider where T : Object
{
	private static readonly string MetadataExtension = ".meta.json";

	private static readonly SerializedObject EmptyMetadata = new SerializedObject();

	private readonly ModRepository _modRepository;

	private readonly SerializedObjectReaderWriter _serializedObjectReaderWriter;

	private readonly IModFileConverter<T> _modFileConverter;

	private readonly Dictionary<string, List<OrderedFile>> _filePaths = new Dictionary<string, List<OrderedFile>>();

	private readonly Dictionary<string, OrderedAsset> _cache = new Dictionary<string, OrderedAsset>();

	public bool IsBuiltIn => false;

	protected ModSystemFileProvider(ModRepository modRepository, SerializedObjectReaderWriter serializedObjectReaderWriter, IModFileConverter<T> modFileConverter)
	{
		_modRepository = modRepository;
		_serializedObjectReaderWriter = serializedObjectReaderWriter;
		_modFileConverter = modFileConverter;
	}

	public void Load()
	{
		foreach (Mod enabledMod in _modRepository.EnabledMods)
		{
			CacheFilesFromMod(enabledMod.ModDirectory.Directory, enabledMod);
		}
	}

	public bool TryLoad<TU>(string path, out OrderedAsset orderedAsset) where TU : Object
	{
		if (typeof(T) == typeof(TU) && _filePaths.TryGetValue(path, out var value))
		{
			List<OrderedFile> list = value;
			if (TryGetFromCacheOrConvert(list[list.Count - 1], path, out var asset))
			{
				orderedAsset = asset;
				return true;
			}
		}
		orderedAsset = default(OrderedAsset);
		return false;
	}

	public IEnumerable<OrderedAsset> LoadAll<TU>(string path, IEnumerable<string> resourceAssets) where TU : Object
	{
		if (!(typeof(T) == typeof(TU)))
		{
			yield break;
		}
		foreach (var (key, list2) in _filePaths)
		{
			if (!key.StartsWith(path))
			{
				continue;
			}
			foreach (OrderedFile item in list2)
			{
				if (TryGetFromCacheOrConvert(item, key, out var asset))
				{
					yield return asset;
				}
			}
		}
	}

	public void Reset()
	{
		_cache.Clear();
		_modFileConverter.Reset();
	}

	private void CacheFilesFromMod(DirectoryInfo modDirectory, Mod mod)
	{
		FileInfo[] files = modDirectory.GetFiles("*", SearchOption.AllDirectories);
		foreach (FileInfo fileInfo in files)
		{
			if (_modFileConverter.CanConvert(fileInfo))
			{
				string key = AssetPathHelper.NormalizeAssetPath(fileInfo.FullName, modDirectory.FullName);
				if (!_filePaths.ContainsKey(key))
				{
					_filePaths[key] = new List<OrderedFile>();
				}
				_filePaths[key].Add(new OrderedFile(_modRepository.Mods.IndexOf(mod), fileInfo, mod.DisplayName));
			}
		}
	}

	private bool TryGetFromCacheOrConvert(OrderedFile orderedFile, string path, out OrderedAsset asset)
	{
		FileInfo file = orderedFile.File;
		if (_cache.TryGetValue(file.FullName, out var value))
		{
			asset = value;
			return true;
		}
		if (_modFileConverter.TryConvert(orderedFile, path, GetMetadata(file), out var asset2))
		{
			asset = new OrderedAsset(orderedFile.Order, asset2);
			_cache.Add(file.FullName, asset);
			return true;
		}
		Debug.LogWarning("Failed to convert " + file.FullName + " to " + typeof(T).Name);
		asset = default(OrderedAsset);
		return false;
	}

	private SerializedObject GetMetadata(FileInfo fileInfo)
	{
		string path = fileInfo.FullName + MetadataExtension;
		if (File.Exists(path))
		{
			using (FileStream stream = File.OpenRead(path))
			{
				return _serializedObjectReaderWriter.ReadJson(stream);
			}
		}
		return EmptyMetadata;
	}
}
