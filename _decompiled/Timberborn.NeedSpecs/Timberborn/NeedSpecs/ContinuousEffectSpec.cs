using Timberborn.BlueprintSystem;

namespace Timberborn.NeedSpecs;

public record ContinuousEffectSpec
{
	[Serialize]
	public string NeedId { get; init; }

	[Serialize]
	public float PointsPerHour { get; init; }

	[Serialize]
	public bool SatisfyToMaxValue { get; init; }
}
