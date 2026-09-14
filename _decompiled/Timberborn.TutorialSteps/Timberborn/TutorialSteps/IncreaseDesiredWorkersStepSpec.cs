using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record IncreaseDesiredWorkersStepSpec : ComponentSpec
{
	[Serialize]
	public string TemplateName { get; init; }
}
