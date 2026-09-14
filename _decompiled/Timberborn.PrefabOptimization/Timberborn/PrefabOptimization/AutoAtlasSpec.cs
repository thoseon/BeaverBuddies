using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

internal record AutoAtlasSpec
{
	[Serialize]
	public string Name { get; init; }

	[Serialize]
	public bool IsUnique { get; init; }

	[Serialize]
	public ImmutableArray<AssetRef<Material>> Fragments { get; init; }
}
