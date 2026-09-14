using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.StockpileVisualization;

internal record StockpileGoodPileVisualizerSpec : ComponentSpec
{
	[Serialize]
	public Vector3 CenterOffset { get; init; }

	[Serialize]
	public ImmutableArray<string> GoodPileVisualizations { get; init; }
}
