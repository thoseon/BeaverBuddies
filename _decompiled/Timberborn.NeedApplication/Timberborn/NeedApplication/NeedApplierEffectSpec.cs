using Timberborn.BlueprintSystem;
using Timberborn.Effects;

namespace Timberborn.NeedApplication;

public record NeedApplierEffectSpec
{
	[Serialize]
	public string NeedId { get; init; }

	[Serialize]
	public float Points { get; init; }

	[Serialize]
	public EffectProbability Probability { get; init; }

	public InstantEffect ToInstantEffect()
	{
		return new InstantEffect(NeedId, Points, 1);
	}
}
