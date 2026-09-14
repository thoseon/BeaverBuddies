using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.TutorialSteps;

internal record TutorialColorsSpec : ComponentSpec
{
	[Serialize]
	public Color TutorialBuildingHighlight { get; init; }
}
