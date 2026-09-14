using Timberborn.BlueprintSystem;

namespace Timberborn.WalkingSystem;

internal record RunningStateUpdaterSpec : ComponentSpec
{
	[Serialize]
	public float ShortWalkingDistanceThreshold { get; init; }

	[Serialize]
	public float WalkingSpeedThreshold { get; init; }
}
