using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.TimbermeshMaterials;

public record MaterialCollectionSpec : ComponentSpec
{
	[Serialize]
	public string CollectionId { get; init; }

	[Serialize]
	public ImmutableArray<AssetRef<Material>> Materials { get; init; }
}
