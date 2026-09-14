using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.Ruins;

internal record RuinModelFactorySpec : ComponentSpec
{
	[Serialize]
	public AssetRef<GameObject> IvyDryModel { get; init; }

	[Serialize]
	public AssetRef<GameObject> IvyWetModel { get; init; }

	[Serialize]
	public ImmutableArray<RuinModelVariantSpec> RuinModelVariants { get; init; }
}
