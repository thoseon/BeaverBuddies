using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.GoodConsumingBuildingSystem;

internal record GoodConsumingBuildingSpec : ComponentSpec
{
	[Serialize]
	public int FullInventoryWorkHours { get; init; }

	[Serialize]
	public ImmutableArray<ConsumedGoodSpec> ConsumedGoods { get; init; }
}
