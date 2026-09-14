using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record ConnectBuildingsTutorialStepSpec : ComponentSpec
{
	[Serialize]
	public string TemplateName { get; init; }

	[Serialize]
	public int RequiredAmount { get; init; }

	[Serialize]
	public bool CountUnfinishedBuildings { get; init; }

	[Serialize]
	public ImmutableArray<string> HighlightableBuildingIds { get; init; }
}
