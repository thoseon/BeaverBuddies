using System.Collections.Immutable;
using System.Linq;

namespace Timberborn.BlueprintSystem;

public class BlueprintFileBundle
{
	public string Name { get; }

	public string Path { get; }

	public ImmutableArray<string> Jsons { get; }

	public ImmutableArray<string> Sources { get; }

	private BlueprintFileBundle(string name, string path, ImmutableArray<string> jsons, ImmutableArray<string> sources)
	{
		Name = name;
		Path = path;
		Jsons = jsons;
		Sources = sources;
	}

	public static BlueprintFileBundle CreateBundled(IGrouping<string, BlueprintAsset> assets)
	{
		return new BlueprintFileBundle(assets.First().Name, assets.Key, assets.Select((BlueprintAsset asset) => asset.Content).ToImmutableArray(), assets.Select((BlueprintAsset asset) => asset.Source).ToImmutableArray());
	}

	public static BlueprintFileBundle CreateSingle(BlueprintAsset asset)
	{
		return new BlueprintFileBundle(asset.Name, asset.Path, ImmutableArray.Create(asset.Content), ImmutableArray.Create(asset.Source));
	}

	public BlueprintFileBundle AddJson(string json, string source)
	{
		return new BlueprintFileBundle(Name, Path, Jsons.Add(json), Sources.Add(source));
	}
}
