using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

internal record FlowSensorSpec : ComponentSpec
{
	[Serialize]
	public Vector3Int SensorCoordinates { get; init; }

	[Serialize]
	public float MaxThreshold { get; init; }
}
