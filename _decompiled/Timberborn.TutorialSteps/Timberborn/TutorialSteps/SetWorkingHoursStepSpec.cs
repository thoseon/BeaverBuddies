using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record SetWorkingHoursStepSpec : ComponentSpec
{
	[Serialize]
	public int TargetWorkingHours { get; init; }
}
