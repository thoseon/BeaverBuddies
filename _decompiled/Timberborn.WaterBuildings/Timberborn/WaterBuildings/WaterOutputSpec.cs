using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal record WaterOutputSpec : ComponentSpec
{
	[Serialize]
	public Vector3Int WaterCoordinates { get; init; }

	[Serialize]
	public float DistanceToGroundOffset { get; init; }

	[Serialize]
	public bool OverflowAllowed { get; init; }
}
