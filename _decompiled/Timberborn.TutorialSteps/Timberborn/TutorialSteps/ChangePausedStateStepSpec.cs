using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record ChangePausedStateStepSpec : ComponentSpec
{
	[Serialize]
	public bool ShouldBePaused { get; init; }

	[Serialize]
	public string TemplateName { get; init; }
}
