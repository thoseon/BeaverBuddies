using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record PowerBuildingsTutorialStepSpec : ComponentSpec
{
	[Serialize]
	public string TemplateName { get; init; }

	[Serialize]
	public int RequiredAmount { get; init; }
}
