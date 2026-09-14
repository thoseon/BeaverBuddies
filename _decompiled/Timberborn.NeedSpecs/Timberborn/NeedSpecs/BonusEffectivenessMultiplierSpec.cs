using Timberborn.BlueprintSystem;

namespace Timberborn.NeedSpecs;

public record BonusEffectivenessMultiplierSpec
{
	[Serialize]
	public string BonusId { get; init; }

	[Serialize]
	public float EffectivenessMultiplier { get; init; }
}
