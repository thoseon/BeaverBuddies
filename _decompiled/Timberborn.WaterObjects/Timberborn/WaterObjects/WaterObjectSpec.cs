using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WaterObjects;

public record WaterObjectSpec : ComponentSpec, IWaterObjectSpecification
{
	[Serialize]
	public Vector3Int WaterCoordinates { get; init; }
}
