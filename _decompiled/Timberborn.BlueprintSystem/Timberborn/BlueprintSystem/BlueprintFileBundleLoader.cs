using System.Collections.Generic;
using System.Linq;
using Timberborn.AssetSystem;

namespace Timberborn.BlueprintSystem;

public class BlueprintFileBundleLoader
{
	private readonly IAssetLoader _assetLoader;

	public BlueprintFileBundleLoader(IAssetLoader assetLoader)
	{
		_assetLoader = assetLoader;
	}

	public IEnumerable<BlueprintFileBundle> GetBundles(string rootPath, IEnumerable<string> blueprintPaths = null)
	{
		return (from asset in _assetLoader.LoadAll<BlueprintAsset>(rootPath, blueprintPaths)
			group asset.Asset by asset.Asset.Path.Replace(BlueprintAsset.OptionalExtension, BlueprintAsset.Extension) into assetGroup
			where assetGroup.Any((BlueprintAsset asset) => !asset.Path.EndsWith(BlueprintAsset.OptionalExtension))
			select assetGroup).Select(BlueprintFileBundle.CreateBundled);
	}

	public BlueprintFileBundle GetBundle(string path)
	{
		IGrouping<string, BlueprintAsset> grouping = (from asset in _assetLoader.LoadAll<BlueprintAsset>(path).Concat(_assetLoader.LoadAll<BlueprintAsset>(path.Replace(BlueprintAsset.Extension, BlueprintAsset.OptionalExtension)))
			group asset.Asset by asset.Asset.Path.Replace(BlueprintAsset.OptionalExtension, BlueprintAsset.Extension)).SingleOrDefault();
		if (grouping == null || grouping.All((BlueprintAsset asset) => asset.Path.EndsWith(BlueprintAsset.OptionalExtension)))
		{
			return null;
		}
		return BlueprintFileBundle.CreateBundled(grouping);
	}
}
