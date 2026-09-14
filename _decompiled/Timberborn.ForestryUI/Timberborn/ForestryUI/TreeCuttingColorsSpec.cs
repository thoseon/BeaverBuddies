using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.ForestryUI;

internal record TreeCuttingColorsSpec : ComponentSpec
{
	[Serialize]
	public Color ToolActionTile { get; init; }

	[Serialize]
	public Color ToolNoActionTile { get; init; }

	[Serialize]
	public Color CuttingAreaTile { get; init; }

	[Serialize]
	public Color CuttingAreaHighlight { get; init; }
}
