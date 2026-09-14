using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.PowerGeneration;

internal record WaterPoweredGeneratorSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<Vector2Int> Blocks { get; init; }

	[Serialize]
	public Vector2 ExpectedWaterDirection { get; init; }

	[Serialize]
	public float MinRequiredOutflow { get; init; }
}
