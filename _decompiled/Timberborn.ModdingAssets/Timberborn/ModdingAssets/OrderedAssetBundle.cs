using UnityEngine;

namespace Timberborn.ModdingAssets;

public readonly struct OrderedAssetBundle
{
	public int Order { get; }

	public AssetBundle AssetBundle { get; }

	public OrderedAssetBundle(int order, AssetBundle assetBundle)
	{
		Order = order;
		AssetBundle = assetBundle;
	}
}
