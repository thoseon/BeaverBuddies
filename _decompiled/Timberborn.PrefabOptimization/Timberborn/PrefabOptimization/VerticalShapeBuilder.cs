using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

public class VerticalShapeBuilder
{
	private readonly IPrefabOptimizationChain _prefabOptimizationChain;

	private readonly OptimizedPrefabInstantiator _optimizedPrefabInstantiator;

	private readonly MeshBuilder _meshBuilder = new MeshBuilder();

	private readonly Dictionary<VerticalShapeInfo, GameObject> _shapeInfoCache = new Dictionary<VerticalShapeInfo, GameObject>();

	public VerticalShapeBuilder(IPrefabOptimizationChain prefabOptimizationChain, OptimizedPrefabInstantiator optimizedPrefabInstantiator)
	{
		_prefabOptimizationChain = prefabOptimizationChain;
		_optimizedPrefabInstantiator = optimizedPrefabInstantiator;
	}

	public GameObject Build(Transform parent, VerticalShapeInfo shapeInfo)
	{
		GameObject orCreateShapePrefab = GetOrCreateShapePrefab(shapeInfo);
		return _optimizedPrefabInstantiator.Instantiate(orCreateShapePrefab, parent);
	}

	private GameObject GetOrCreateShapePrefab(VerticalShapeInfo shapeInfo)
	{
		if (!_shapeInfoCache.TryGetValue(shapeInfo, out var value))
		{
			CreateMeshShape(shapeInfo);
			_shapeInfoCache.Add(shapeInfo, value = BuildPrefab());
		}
		return value;
	}

	private void CreateMeshShape(VerticalShapeInfo shapeInfo)
	{
		_meshBuilder.Reset(shapeInfo.Name);
		BuiltMesh meshAndMaterials = GetMeshAndMaterials(shapeInfo.StartPrefab);
		BuiltMesh meshAndMaterials2 = GetMeshAndMaterials(shapeInfo.RepeatingPrefab);
		for (int i = 0; i < shapeInfo.TotalPrefabCount; i++)
		{
			TranslationTransform transform = new TranslationTransform(i * Vector3.down);
			_meshBuilder.AppendMesh((i == 0) ? meshAndMaterials : meshAndMaterials2, transform);
		}
	}

	private static BuiltMesh GetMeshAndMaterials(GameObject source)
	{
		MeshRenderer componentInChildren = source.GetComponentInChildren<MeshRenderer>();
		return new BuiltMesh(source.GetComponentInChildren<MeshFilter>().sharedMesh, componentInChildren.sharedMaterials);
	}

	private GameObject BuildPrefab()
	{
		BuiltMesh builtMesh = _meshBuilder.Build();
		GameObject gameObject = new GameObject(builtMesh.Mesh.name);
		gameObject.AddComponent<MeshRenderer>().sharedMaterials = builtMesh.Materials;
		gameObject.AddComponent<MeshFilter>().sharedMesh = builtMesh.Mesh;
		GameObject result = _prefabOptimizationChain.Process(gameObject);
		Object.Destroy(gameObject);
		return result;
	}
}
