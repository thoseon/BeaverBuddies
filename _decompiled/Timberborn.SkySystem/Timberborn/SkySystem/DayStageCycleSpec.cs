using Timberborn.BlueprintSystem;

namespace Timberborn.SkySystem;

internal record DayStageCycleSpec : ComponentSpec
{
	[Serialize]
	public float SunriseSunsetLengthInHours { get; init; }

	[Serialize]
	public float TransitionLengthInHours { get; init; }
}
