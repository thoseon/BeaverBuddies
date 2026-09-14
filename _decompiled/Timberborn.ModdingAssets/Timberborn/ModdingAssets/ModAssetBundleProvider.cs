using System.Collections.Generic;
using Timberborn.AssetSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ModdingAssets;

internal class ModAssetBundleProvider : ILoadableSingleton, IAssetProvider
{
	private readonly struct AssetBundleAssetPath(AssetBundle assetBundle, string assetPath, int order)
	{
		private readonly AssetBundle _assetBundle = assetBundle;

		private readonly string _assetPath = assetPath;

		private readonly int _order = order;

		public bool TryLoad<T>(out OrderedAsset orderedAsset) where T : Object
		{
			T val = _assetBundle.LoadAsset<T>(_assetPath);
			if (!val && typeof(Component).IsAssignableFrom(typeof(T)))
			{
				GameObject gameObject = _assetBundle.LoadAsset<GameObject>(_assetPath);
				if ((bool)gameObject)
				{
					val = gameObject.GetComponent<T>();
				}
			}
			orderedAsset = new OrderedAsset(_order, val);
			return val;
		}
	}

	private readonly ModAssetBundleLoader _modAssetBundleLoader;

	private readonly Dictionary<string, List<AssetBundleAssetPath>> _assetPaths = new Dictionary<string, List<AssetBundleAssetPath>>();

	public bool IsBuiltIn => false;

	public ModAssetBundleProvider(ModAssetBundleLoader modAssetBundleLoader)
	{
		_modAssetBundleLoader = modAssetBundleLoader;
	}

	public void Load()
	{
		CachePaths();
	}

	public bool TryLoad<T>(string path, out OrderedAsset orderedAsset) where T : Object
	{
		if (_assetPaths.TryGetValue(path, out var value))
		{
			List<AssetBundleAssetPath> list = value;
			if (list[list.Count - 1].TryLoad<T>(out orderedAsset))
			{
				return true;
			}
		}
		orderedAsset = default(OrderedAsset);
		return false;
	}

	public IEnumerable<OrderedAsset> LoadAll<T>(string path, IEnumerable<string> resourceAssets) where T : Object
	{
		foreach (var (text2, list2) in _assetPaths)
		{
			if (!text2.StartsWith(path))
			{
				continue;
			}
			foreach (AssetBundleAssetPath item in list2)
			{
				if (item.TryLoad<T>(out var orderedAsset))
				{
					yield return orderedAsset;
				}
			}
		}
	}

	public void Reset()
	{
		_assetPaths.Clear();
		_modAssetBundleLoader.Reload();
		CachePaths();
	}

	private void CachePaths()
	{
		foreach (OrderedAssetBundle loadedAssetBundle in _modAssetBundleLoader.LoadedAssetBundles)
		{
			string[] allAssetNames = loadedAssetBundle.AssetBundle.GetAllAssetNames();
			foreach (string assetPath in allAssetNames)
			{
				string key = AssetPathHelper.NormalizeAssetPath(assetPath);
				if (!_assetPaths.ContainsKey(key))
				{
					_assetPaths[key] = new List<AssetBundleAssetPath>();
				}
				_assetPaths[key].Add(new AssetBundleAssetPath(loadedAssetBundle.AssetBundle, assetPath, loadedAssetBundle.Order));
			}
		}
	}
}
