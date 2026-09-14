using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintPrefabSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

internal class PrefabOptimizationChain : IPrefabOptimizationChain
{
	private static readonly string RootGameObjectName = "OptimizedPrefabs";

	private readonly List<IPrefabOptimizer> _prefabProcessors;

	private readonly BlueprintPrefabConverter _blueprintPrefabConverter;

	private readonly Dictionary<GameObject, GameObject> _prefabCache = new Dictionary<GameObject, GameObject>();

	private readonly Dictionary<Blueprint, GameObject> _blueprintCache = new Dictionary<Blueprint, GameObject>();

	private readonly Lazy<GameObject> _rootGameObject = new Lazy<GameObject>(CreateRootGameObject);

	public PrefabOptimizationChain(IEnumerable<IPrefabOptimizer> prefabProcessors, BlueprintPrefabConverter blueprintPrefabConverter)
	{
		_prefabProcessors = prefabProcessors.ToList();
		_blueprintPrefabConverter = blueprintPrefabConverter;
	}

	public GameObject Process(GameObject inputPrefab)
	{
		if (!_prefabCache.ContainsKey(inputPrefab))
		{
			GameObject value = ProcessPrefab(inputPrefab);
			_prefabCache.Add(inputPrefab, value);
		}
		return _prefabCache[inputPrefab];
	}

	public GameObject Process(Blueprint inputBlueprint)
	{
		if (!_blueprintCache.ContainsKey(inputBlueprint))
		{
			GameObject value = ProcessPrefab(inputBlueprint);
			_blueprintCache.Add(inputBlueprint, value);
		}
		return _blueprintCache[inputBlueprint];
	}

	public ImmutableArray<GameObject> GetCached()
	{
		return _prefabCache.Values.ToImmutableArray();
	}

	private static GameObject CreateRootGameObject()
	{
		GameObject gameObject = new GameObject(RootGameObjectName);
		gameObject.SetActive(value: false);
		return gameObject;
	}

	private GameObject ProcessPrefab(GameObject inputPrefab)
	{
		if (_prefabProcessors.IsEmpty())
		{
			return inputPrefab;
		}
		try
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(inputPrefab, _rootGameObject.Value.transform);
			gameObject.name = inputPrefab.name;
			foreach (IPrefabOptimizer prefabProcessor in _prefabProcessors)
			{
				prefabProcessor.Optimize(gameObject);
			}
			return gameObject;
		}
		catch (Exception innerException)
		{
			throw new Exception("Processing prefab " + inputPrefab.name + " failed.", innerException);
		}
	}

	private GameObject ProcessPrefab(Blueprint inputBlueprint)
	{
		return ProcessPrefab(_blueprintPrefabConverter.Convert(inputBlueprint, _rootGameObject.Value.transform));
	}
}
