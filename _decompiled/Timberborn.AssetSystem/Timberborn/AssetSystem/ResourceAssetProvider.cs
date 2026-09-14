using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Timberborn.AssetSystem;

public class ResourceAssetProvider : IAssetProvider
{
	private static readonly int ResourceAssetOrder = -1;

	public bool IsBuiltIn => true;

	public bool TryLoad<T>(string path, out OrderedAsset orderedAsset) where T : Object
	{
		orderedAsset = new OrderedAsset(ResourceAssetOrder, Resources.Load<T>(path));
		return orderedAsset.Asset != null;
	}

	public IEnumerable<OrderedAsset> LoadAll<T>(string path, IEnumerable<string> resourceAssets) where T : Object
	{
		if (resourceAssets != null)
		{
			return from asset in resourceAssets
				select Resources.Load<T>(Path.Combine(path, asset)) into asset
				select new OrderedAsset(ResourceAssetOrder, asset);
		}
		return from asset in Resources.LoadAll<T>(path)
			select new OrderedAsset(ResourceAssetOrder, asset);
	}

	public void Reset()
	{
	}
}
