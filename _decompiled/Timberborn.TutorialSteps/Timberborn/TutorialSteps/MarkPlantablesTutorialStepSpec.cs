using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record MarkPlantablesTutorialStepSpec : ComponentSpec
{
	[Serialize]
	public string TemplateName { get; init; }

	[Serialize]
	public int RequiredAmount { get; init; }
}
