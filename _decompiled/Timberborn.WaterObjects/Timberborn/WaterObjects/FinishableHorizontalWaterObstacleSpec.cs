using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WaterObjects;

internal record FinishableHorizontalWaterObstacleSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<Vector3Int> Obstacles { get; init; }
}
