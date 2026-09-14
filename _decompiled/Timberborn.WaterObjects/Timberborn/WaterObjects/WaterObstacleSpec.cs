using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WaterObjects;

internal record WaterObstacleSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<Vector2Int> Coordinates { get; init; }
}
