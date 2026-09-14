using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.Ruins;

internal record RuinModelVariantSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public AssetRef<GameObject> Model { get; init; }
}
