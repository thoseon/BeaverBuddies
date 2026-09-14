using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.MapEditorNaturalResourcesUI;

internal record NaturalResourceBrushSpec : ComponentSpec
{
	[Serialize]
	public Color RemovalTileColor { get; init; }

	[Serialize]
	public Color SpawnTileColor { get; init; }

	[Serialize]
	public string DefaultNaturalResourceId { get; init; }
}
