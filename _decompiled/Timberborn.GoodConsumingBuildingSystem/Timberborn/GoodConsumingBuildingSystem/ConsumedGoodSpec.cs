using Timberborn.BlueprintSystem;

namespace Timberborn.GoodConsumingBuildingSystem;

public record ConsumedGoodSpec : ComponentSpec
{
	[Serialize]
	public string GoodId { get; init; }

	[Serialize]
	public float GoodPerHour { get; init; }
}
