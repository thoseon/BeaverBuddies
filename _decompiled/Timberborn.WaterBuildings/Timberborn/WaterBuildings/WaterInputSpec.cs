using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WaterBuildings;

public record WaterInputSpec : ComponentSpec
{
	[Serialize]
	public Vector3Int WaterInputCoordinates { get; init; }
}
