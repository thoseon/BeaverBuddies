using Timberborn.BlueprintSystem;

namespace Timberborn.BonusSystem;

public record BonusSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public float MultiplierDelta { get; init; }
}
