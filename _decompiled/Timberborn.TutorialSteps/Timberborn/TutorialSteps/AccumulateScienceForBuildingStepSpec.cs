using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record AccumulateScienceForBuildingStepSpec : ComponentSpec
{
	[Serialize]
	public string TemplateName { get; init; }
}
