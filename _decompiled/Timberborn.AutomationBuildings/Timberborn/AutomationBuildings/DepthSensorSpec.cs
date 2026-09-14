using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

internal record DepthSensorSpec : ComponentSpec
{
	[Serialize]
	public Vector3Int SensorCoordinates { get; init; }

	[Serialize]
	public float SensorHeightOffset { get; init; }
}
