using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record SetPauseStepSpec : ComponentSpec
{
	[Serialize]
	public bool Pause { get; init; }

	[Serialize]
	public bool OnlyOnce { get; init; }
}
