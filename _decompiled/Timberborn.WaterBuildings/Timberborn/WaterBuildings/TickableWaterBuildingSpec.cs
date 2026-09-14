using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal record TickableWaterBuildingSpec : ComponentSpec
{
	[Serialize]
	public Vector3Int WaterCoordinates { get; init; }

	[Serialize]
	public float MinWaterHeight { get; init; }

	[Serialize]
	public float ChangeRange { get; init; }
}
