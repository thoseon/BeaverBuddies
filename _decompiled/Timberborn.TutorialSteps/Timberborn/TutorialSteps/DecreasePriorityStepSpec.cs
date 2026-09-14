using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record DecreasePriorityStepSpec : ComponentSpec
{
	[Serialize]
	public string TemplateName { get; init; }
}
