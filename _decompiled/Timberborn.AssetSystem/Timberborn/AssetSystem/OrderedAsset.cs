using UnityEngine;

namespace Timberborn.AssetSystem;

public readonly struct OrderedAsset
{
	public int Order { get; }

	public Object Asset { get; }

	public OrderedAsset(int order, Object asset)
	{
		Order = order;
		Asset = asset;
	}
}
