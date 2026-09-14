using System.Collections.Generic;
using Bindito.Core;
using Timberborn.BlueprintPrefabSystem;

namespace Timberborn.PrefabOptimization;

internal class PrefabOptimizationChainProvider : IProvider<IPrefabOptimizationChain>
{
	private readonly TimbermeshPrefabOptimizer _timbermeshPrefabOptimizer;

	private readonly AutoAtlasingPrefabOptimizer _autoAtlasingPrefabOptimizer;

	private readonly VertexColorPrefabOptimizer _vertexColorPrefabOptimizer;

	private readonly MergeMeshesByMaterialPrefabOptimizer _mergeMeshesByMaterialPrefabOptimizer;

	private readonly DestroyEmptyChildrenPrefabOptimizer _destroyEmptyChildrenPrefabOptimizer;

	private readonly BlueprintPrefabConverter _blueprintPrefabConverter;

	public PrefabOptimizationChainProvider(TimbermeshPrefabOptimizer timbermeshPrefabOptimizer, AutoAtlasingPrefabOptimizer autoAtlasingPrefabOptimizer, VertexColorPrefabOptimizer vertexColorPrefabOptimizer, MergeMeshesByMaterialPrefabOptimizer mergeMeshesByMaterialPrefabOptimizer, DestroyEmptyChildrenPrefabOptimizer destroyEmptyChildrenPrefabOptimizer, BlueprintPrefabConverter blueprintPrefabConverter)
	{
		_timbermeshPrefabOptimizer = timbermeshPrefabOptimizer;
		_autoAtlasingPrefabOptimizer = autoAtlasingPrefabOptimizer;
		_vertexColorPrefabOptimizer = vertexColorPrefabOptimizer;
		_mergeMeshesByMaterialPrefabOptimizer = mergeMeshesByMaterialPrefabOptimizer;
		_destroyEmptyChildrenPrefabOptimizer = destroyEmptyChildrenPrefabOptimizer;
		_blueprintPrefabConverter = blueprintPrefabConverter;
	}

	public IPrefabOptimizationChain Get()
	{
		List<IPrefabOptimizer> list = new List<IPrefabOptimizer> { _timbermeshPrefabOptimizer };
		if (PrefabOptimizationChainConfiguration.AutoAtlasing)
		{
			list.Add(_autoAtlasingPrefabOptimizer);
		}
		if (PrefabOptimizationChainConfiguration.VertexColor)
		{
			list.Add(_vertexColorPrefabOptimizer);
		}
		if (PrefabOptimizationChainConfiguration.MergeMeshesByMaterial)
		{
			list.Add(_mergeMeshesByMaterialPrefabOptimizer);
		}
		if (PrefabOptimizationChainConfiguration.DestroyEmptyChildren)
		{
			list.Add(_destroyEmptyChildrenPrefabOptimizer);
		}
		return new PrefabOptimizationChain(list, _blueprintPrefabConverter);
	}
}
