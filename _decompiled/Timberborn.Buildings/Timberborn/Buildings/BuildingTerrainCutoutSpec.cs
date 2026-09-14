using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.Buildings;

internal record BuildingTerrainCutoutSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<Vector3Int> CutoutTiles { get; init; }
}
