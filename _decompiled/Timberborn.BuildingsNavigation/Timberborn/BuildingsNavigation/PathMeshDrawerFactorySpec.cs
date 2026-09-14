using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal record PathMeshDrawerFactorySpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<AssetRef<Mesh>> RegularModelVariants { get; init; }

	[Serialize]
	public ImmutableArray<AssetRef<Mesh>> StairsModelVariants { get; init; }

	[Serialize]
	public AssetRef<Material> Material { get; init; }
}
