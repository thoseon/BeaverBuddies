using Timberborn.BlueprintSystem;

namespace Timberborn.NeedSpecs;

public record InstantEffectSpec
{
	[Serialize]
	public string NeedId { get; init; }

	[Serialize]
	public float Points { get; init; }
}
