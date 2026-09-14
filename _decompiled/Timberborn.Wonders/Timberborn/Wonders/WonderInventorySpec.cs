using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.Goods;

namespace Timberborn.Wonders;

internal record WonderInventorySpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<GoodAmountSpec> RequiredGoods { get; init; }
}
