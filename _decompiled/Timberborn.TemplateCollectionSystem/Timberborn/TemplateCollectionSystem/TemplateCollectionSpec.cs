using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.TemplateCollectionSystem;

public record TemplateCollectionSpec : ComponentSpec
{
	[Serialize]
	public string CollectionId { get; init; }

	[Serialize]
	public ImmutableArray<AssetRef<BlueprintAsset>> Blueprints { get; init; }
}
