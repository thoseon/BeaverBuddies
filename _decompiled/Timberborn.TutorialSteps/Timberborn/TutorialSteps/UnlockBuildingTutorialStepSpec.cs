using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record UnlockBuildingTutorialStepSpec : ComponentSpec
{
	[Serialize]
	public string TemplateName { get; init; }
}
