using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.StockpileVisualization;

internal record StockpilePlaneVisualizerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<StockpilePlaneVisualization> StockpilePlaneVisualizations { get; init; }
}
