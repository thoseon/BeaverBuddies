using Bindito.Unity;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

public class OptimizedPrefabInstantiator
{
	private readonly IInstantiator _instantiator;

	private readonly IPrefabOptimizationChain _prefabOptimizationChain;

	public OptimizedPrefabInstantiator(IInstantiator instantiator, IPrefabOptimizationChain prefabOptimizationChain)
	{
		_instantiator = instantiator;
		_prefabOptimizationChain = prefabOptimizationChain;
	}

	public GameObject Instantiate(GameObject prefab, Transform parent)
	{
		GameObject prefab2 = _prefabOptimizationChain.Process(prefab);
		return _instantiator.Instantiate(prefab2, parent);
	}

	public GameObject InstantiateInactive(Blueprint blueprint, Transform parent)
	{
		GameObject prefab = _prefabOptimizationChain.Process(blueprint);
		bool wasActive;
		return _instantiator.InstantiateInactive(prefab, parent, out wasActive);
	}
}
