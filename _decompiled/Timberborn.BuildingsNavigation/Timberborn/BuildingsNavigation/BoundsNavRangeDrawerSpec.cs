using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal record BoundsNavRangeDrawerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<AssetRef<Mesh>> TileMeshes { get; init; }

	[Serialize]
	public AssetRef<Material> Material { get; init; }
}
