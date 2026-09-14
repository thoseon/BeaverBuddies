using Timberborn.BlueprintSystem;

namespace Timberborn.Stockpiles;

public record StockpileSpec : ComponentSpec
{
	[Serialize]
	public int MaxCapacity { get; init; }

	[Serialize]
	public string WhitelistedGoodType { get; init; }
}
