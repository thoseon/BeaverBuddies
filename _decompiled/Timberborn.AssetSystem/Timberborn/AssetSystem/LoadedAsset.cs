namespace Timberborn.AssetSystem;

public readonly struct LoadedAsset<T>
{
	public T Asset { get; }

	public bool IsBuiltIn { get; }

	public int Order { get; }

	public LoadedAsset(T asset, bool isBuiltIn, int order)
	{
		Asset = asset;
		IsBuiltIn = isBuiltIn;
		Order = order;
	}
}
