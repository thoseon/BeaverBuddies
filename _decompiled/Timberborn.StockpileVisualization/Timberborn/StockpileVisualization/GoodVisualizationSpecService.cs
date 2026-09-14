using System.Collections.Frozen;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.StockpileVisualization;

public class GoodVisualizationSpecService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private FrozenDictionary<string, GoodVisualizationSpec> _goodVisualizationSpecs;

	public GoodVisualizationSpecService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		_goodVisualizationSpecs = _specService.GetSpecs<GoodVisualizationSpec>().ToFrozenDictionary((GoodVisualizationSpec spec) => spec.Id + spec.Variant, (GoodVisualizationSpec spec) => spec);
	}

	public GoodVisualizationSpec GetVisualization(string visualizationId, string visualizationVariant = "")
	{
		return _goodVisualizationSpecs[visualizationId + visualizationVariant];
	}
}
