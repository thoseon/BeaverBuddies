using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.Terraforming;

internal record DrillSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<Vector3Int> DrillableCoordinates { get; init; }

	[Serialize]
	public string RemovalEffectPath { get; init; }
}
