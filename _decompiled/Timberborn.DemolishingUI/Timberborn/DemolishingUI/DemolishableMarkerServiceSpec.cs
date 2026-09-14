using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.DemolishingUI;

internal record DemolishableMarkerServiceSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<Mesh> Mesh { get; init; }

	[Serialize]
	public AssetRef<Material> Material { get; init; }
}
