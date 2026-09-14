using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.NeedCollectionSystem;

public record NeedCollectionSpec : ComponentSpec
{
	[Serialize]
	public string CollectionId { get; init; }

	[Serialize]
	public ImmutableArray<string> Needs { get; init; }
}
