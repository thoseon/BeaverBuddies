using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record VisibleLevelChangeStepSpec : ComponentSpec
{
	[Serialize]
	public VisibleLevelChangeType VisibleLevelChangeType { get; init; }

	[Serialize]
	public bool ShowKeybindings { get; init; }
}
