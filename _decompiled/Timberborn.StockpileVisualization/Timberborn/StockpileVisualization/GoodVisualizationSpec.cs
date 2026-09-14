using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.StockpileVisualization;

public record GoodVisualizationSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public string Variant { get; init; }

	[Serialize]
	public Vector3 Offset { get; init; }

	[Serialize]
	public float LimitingAmount { get; init; }

	[Serialize]
	public AssetRef<Mesh> PrimaryMesh { get; init; }

	[Serialize]
	public AssetRef<Mesh> SecondaryMesh { get; init; }

	[Serialize]
	public AssetRef<Material> Material { get; init; }

	[Serialize]
	public float NonLinearity { get; init; }

	public int LimitingAmountFlooredToInt => (int)LimitingAmount;
}
