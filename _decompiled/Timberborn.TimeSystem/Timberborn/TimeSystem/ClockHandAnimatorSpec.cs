using Timberborn.BlueprintSystem;

namespace Timberborn.TimeSystem;

internal record ClockHandAnimatorSpec : ComponentSpec
{
	[Serialize]
	public float AngleOffset { get; init; }

	[Serialize]
	public string HandName { get; init; }
}
