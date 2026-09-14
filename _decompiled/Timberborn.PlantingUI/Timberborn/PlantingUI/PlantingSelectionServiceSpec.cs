using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.PlantingUI;

internal record PlantingSelectionServiceSpec : ComponentSpec
{
	[Serialize]
	public Color PlantingToolTile { get; init; }

	[Serialize]
	public Color ToolActionTile { get; init; }

	[Serialize]
	public Color ToolNoActionTile { get; init; }
}
