using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.DecalSystem;

internal record DecalSpec : ComponentSpec
{
	[Serialize]
	public string FactionId { get; init; }

	[Serialize]
	public string Category { get; init; }

	[Serialize]
	public AssetRef<Texture2D> Texture { get; init; }

	public string Id => Texture.Asset.name;
}
