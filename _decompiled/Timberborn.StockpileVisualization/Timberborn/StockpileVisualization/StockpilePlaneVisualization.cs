using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.StockpileVisualization;

internal record StockpilePlaneVisualization
{
	[Serialize]
	public Vector3 CenterOffset { get; init; }

	[Serialize]
	public Vector2 MovementRange { get; init; }

	[Serialize]
	public string GoodVisualizationId { get; init; }

	[Serialize]
	public string GoodVisualizationVariant { get; init; }
}
