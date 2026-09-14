using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.Buildings;

internal record BuildingDetailTextureSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<Texture> Texture { get; init; }

	[Serialize]
	public Color Color { get; init; }
}
