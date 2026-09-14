using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record GameSpeedStepSpec : ComponentSpec
{
	[Serialize]
	public int Speed { get; init; }

	[Serialize]
	public bool OnlyOnce { get; init; }
}
