using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.Rendering;

internal record AreaTileDrawerFactorySpec : ComponentSpec
{
	[Serialize]
	public AssetRef<Mesh> TileMesh { get; init; }

	[Serialize]
	public AssetRef<Material> TileMaterial { get; init; }
}
