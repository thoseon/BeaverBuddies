using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;

namespace Timberborn.AssetSystem;

public class AssetLoader : IAssetLoader
{
	private readonly ImmutableArray<IAssetProvider> _assetProviders;

	public AssetLoader(IEnumerable<IAssetProvider> assetProviders)
	{
		_assetProviders = assetProviders.ToImmutableArray();
	}

	public T Load<T>(string path) where T : UnityEngine.Object
	{
		T val = LoadSafe<T>(path);
		if (val != null)
		{
			return val;
		}
		throw new InvalidOperationException("Failed to load asset at " + path);
	}

	public T LoadSafe<T>(string path) where T : UnityEngine.Object
	{
		string path2 = AssetPathHelper.NormalizePath(path);
		OrderedAsset? orderedAsset = null;
		foreach (IAssetProvider assetProvider in _assetProviders)
		{
			if (assetProvider.TryLoad<T>(path2, out var orderedAsset2) && (!orderedAsset.HasValue || orderedAsset2.Order > orderedAsset.Value.Order))
			{
				orderedAsset = orderedAsset2;
			}
		}
		return (T)(orderedAsset?.Asset);
	}

	public IEnumerable<LoadedAsset<T>> LoadAll<T>(string path, IEnumerable<string> resourceAssets) where T : UnityEngine.Object
	{
		string normalizedPath = AssetPathHelper.NormalizePath(path);
		return from loadedAsset in _assetProviders.SelectMany((IAssetProvider assetProvider) => CreateLoadedAssets<T>(assetProvider, normalizedPath, resourceAssets))
			orderby loadedAsset.Order
			select loadedAsset;
	}

	public void Reset()
	{
		foreach (IAssetProvider assetProvider in _assetProviders)
		{
			assetProvider.Reset();
		}
	}

	private static IEnumerable<LoadedAsset<T>> CreateLoadedAssets<T>(IAssetProvider assetProvider, string normalizedPath, IEnumerable<string> resourceAssets) where T : UnityEngine.Object
	{
		return from asset in assetProvider.LoadAll<T>(normalizedPath, resourceAssets)
			select new LoadedAsset<T>((T)asset.Asset, assetProvider.IsBuiltIn, asset.Order);
	}
}
