using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.GoodCollectionSystem;

public record GoodCollectionSpec : ComponentSpec
{
	[Serialize]
	public string CollectionId { get; init; }

	[Serialize]
	public ImmutableArray<string> Goods { get; init; }
}
