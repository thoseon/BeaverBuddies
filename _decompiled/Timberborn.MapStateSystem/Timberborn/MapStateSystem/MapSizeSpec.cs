using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.MapStateSystem;

public record MapSizeSpec : ComponentSpec
{
	[Serialize]
	public Vector2Int DefaultMapSize { get; init; }

	[Serialize]
	public int MinMapSize { get; init; }

	[Serialize]
	public int MaxMapSize { get; init; }

	[Serialize]
	public int MaxMapEditorTerrainHeight { get; init; }

	[Serialize]
	public int MaxGameTerrainHeight { get; init; }

	[Serialize]
	public int MaxHeightAboveTerrain { get; init; }
}
