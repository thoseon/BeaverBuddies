using Timberborn.BlueprintSystem;

namespace Timberborn.Wonders;

internal record WonderDeactivationTimerSpec : ComponentSpec
{
	[Serialize]
	public float TimerDelayInHours { get; init; }
}
