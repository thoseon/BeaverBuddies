using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal record FillValveSpec : ComponentSpec
{
	[Serialize]
	public bool DefaultTargetHeightEnabled { get; init; }

	[Serialize]
	public float DefaultTargetHeightOffset { get; init; }

	[Serialize]
	public bool DefaultAutomationTargetHeightEnabled { get; init; }

	[Serialize]
	public float DefaultAutomationTargetHeightOffset { get; init; }

	[Serialize]
	public float OverflowLimit { get; init; }

	[Serialize]
	public Vector3Int OutputCoordinates { get; init; }
}
