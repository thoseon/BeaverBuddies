using Timberborn.BlueprintSystem;

namespace Timberborn.Yielding;

public record YieldRemovalChanceBonusSpec : ComponentSpec
{
	[Serialize]
	public string BonusId { get; init; }

	[Serialize]
	public string GoodId { get; init; }
}
