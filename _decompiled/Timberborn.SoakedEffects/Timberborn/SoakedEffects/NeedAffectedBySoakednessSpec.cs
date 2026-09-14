using Timberborn.BlueprintSystem;

namespace Timberborn.SoakedEffects;

internal record NeedAffectedBySoakednessSpec : ComponentSpec
{
	[Serialize]
	public float PointsPerHour { get; init; }
}
