using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.GameDistricts;

internal record DistrictObstacleSpec : ComponentSpec
{
	[Serialize]
	public Vector3Int CoordinateOffset { get; init; }
}
