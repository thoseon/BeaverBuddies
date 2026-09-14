using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WaterSourceSystem;

internal record WaterSourceSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<Vector2Int> Coordinates { get; init; }

	[Serialize]
	public float DefaultStrength { get; init; }
}
