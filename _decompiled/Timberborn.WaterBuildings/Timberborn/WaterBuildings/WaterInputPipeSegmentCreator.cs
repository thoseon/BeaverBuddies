using Timberborn.AssetSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.PrefabOptimization;
using Timberborn.Rendering;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class WaterInputPipeSegmentCreator : BaseComponent, IAwakableComponent
{
	private static readonly float[] PipeRotations = new float[4] { 0f, 90f, 180f, 270f };

	private readonly OptimizedPrefabInstantiator _optimizedPrefabInstantiator;

	private readonly IAssetLoader _assetLoader;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly MaterialColorer _materialColorer;

	private EntityMaterials _entityMaterials;

	private GameObject _prefab;

	private Transform _parent;

	public WaterInputPipeSegmentCreator(OptimizedPrefabInstantiator optimizedPrefabInstantiator, IAssetLoader assetLoader, IRandomNumberGenerator randomNumberGenerator, MaterialColorer materialColorer)
	{
		_optimizedPrefabInstantiator = optimizedPrefabInstantiator;
		_assetLoader = assetLoader;
		_randomNumberGenerator = randomNumberGenerator;
		_materialColorer = materialColorer;
	}

	public void Awake()
	{
		_entityMaterials = GetComponent<EntityMaterials>();
		WaterInputPipeSpec component = GetComponent<WaterInputPipeSpec>();
		_prefab = _assetLoader.Load<GameObject>(component.PipeSegmentPrefabPath);
		_parent = base.GameObject.FindChildTransform(component.PipeParentName);
	}

	public PipeSegment CreateFinished()
	{
		GameObject gameObject = _optimizedPrefabInstantiator.Instantiate(_prefab, _parent);
		_entityMaterials.AddMaterials(gameObject);
		float enumerableElement = _randomNumberGenerator.GetEnumerableElement(PipeRotations);
		return PipeSegment.Create(gameObject, enumerableElement);
	}

	public PipeSegment CreateUnfinished()
	{
		PipeSegment pipeSegment = CreateFinished();
		_materialColorer.EnableGrayscale(pipeSegment.Root);
		return pipeSegment;
	}
}
