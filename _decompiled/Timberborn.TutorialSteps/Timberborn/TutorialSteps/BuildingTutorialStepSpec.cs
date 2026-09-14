using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record BuildingTutorialStepSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> TemplateNames { get; init; }

	[Serialize]
	public bool OnlyFinishedBuildings { get; init; }

	[Serialize]
	public int RequiredAmount { get; init; }
}
