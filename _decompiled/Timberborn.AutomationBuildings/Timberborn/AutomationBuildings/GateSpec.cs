using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

internal record GateSpec : ComponentSpec
{
	[Serialize]
	public Vector3Int Start { get; init; }

	[Serialize]
	public Vector3Int End { get; init; }
}
