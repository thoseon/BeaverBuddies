using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

internal record ContaminationSensorSpec : ComponentSpec
{
	[Serialize]
	public Vector3Int SensorCoordinates { get; init; }
}
