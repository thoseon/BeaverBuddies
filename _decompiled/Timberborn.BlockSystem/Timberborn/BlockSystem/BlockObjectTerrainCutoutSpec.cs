using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlockSystem;

internal record BlockObjectTerrainCutoutSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<Vector3Int> CutoutTiles { get; init; }
}
