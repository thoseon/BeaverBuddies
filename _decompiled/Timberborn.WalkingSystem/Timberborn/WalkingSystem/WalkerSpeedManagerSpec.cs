using Timberborn.BlueprintSystem;

namespace Timberborn.WalkingSystem;

internal record WalkerSpeedManagerSpec : ComponentSpec
{
	[Serialize]
	public float BaseWalkingSpeed { get; init; }

	[Serialize]
	public float BaseSlowedSpeed { get; init; }
}
