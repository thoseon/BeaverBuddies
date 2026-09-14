using Bindito.Unity;
using Timberborn.AssetSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.PrefabOptimization;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.Terraforming;

internal class DrillScrewBuilder : BaseComponent, IAwakableComponent, IInitializablePreview, IInitializableEntity
{
	private static readonly int PreviewShapeLength = 2;

	private readonly VerticalShapeBuilder _verticalShapeBuilder;

	private readonly Highlighter _highlighter;

	private readonly IInstantiator _instantiator;

	private readonly IPrefabOptimizationChain _prefabOptimizationChain;

	private readonly IAssetLoader _assetLoader;

	private BlockObject _blockObject;

	private EntityMaterials _entityMaterials;

	private DrillScrewBuilderSpec _drillScrewBuilderSpec;

	private DrillScrewRotator _drillScrewRotator;

	private GameObject _screwHeadPrefab;

	private GameObject _screwAxisPrefab;

	private Transform _parent;

	private GameObject _screwInstance;

	public DrillScrewBuilder(VerticalShapeBuilder verticalShapeBuilder, Highlighter highlighter, IInstantiator instantiator, IPrefabOptimizationChain prefabOptimizationChain, IAssetLoader assetLoader)
	{
		_verticalShapeBuilder = verticalShapeBuilder;
		_highlighter = highlighter;
		_instantiator = instantiator;
		_prefabOptimizationChain = prefabOptimizationChain;
		_assetLoader = assetLoader;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_entityMaterials = GetComponent<EntityMaterials>();
		_drillScrewBuilderSpec = GetComponent<DrillScrewBuilderSpec>();
		_drillScrewRotator = GetComponent<DrillScrewRotator>();
		_screwHeadPrefab = _assetLoader.Load<GameObject>(_drillScrewBuilderSpec.ScrewHeadPrefabPath);
		_screwAxisPrefab = _assetLoader.Load<GameObject>(_drillScrewBuilderSpec.ScrewAxisPrefabPath);
		_parent = base.GameObject.FindChildTransform(_drillScrewBuilderSpec.ParentName);
	}

	public void InitializePreview()
	{
		CreateScrewInstance(isPreview: true);
	}

	public void InitializeEntity()
	{
		CreateScrewInstance(isPreview: false);
	}

	private void CreateScrewInstance(bool isPreview)
	{
		if (!_screwInstance)
		{
			_screwInstance = _verticalShapeBuilder.Build(_parent, GetShapeInfo(isPreview));
			SetupScrewInstance();
			_highlighter.ResetAllHighlights(this);
			_entityMaterials.AddMaterials(_screwInstance);
		}
	}

	private VerticalShapeInfo GetShapeInfo(bool isPreview)
	{
		return new VerticalShapeInfo(isPreview ? PreviewShapeLength : GetDistanceToBottomOfMap(), _prefabOptimizationChain.Process(_screwHeadPrefab), _prefabOptimizationChain.Process(_screwAxisPrefab), "DrillScrew");
	}

	private void SetupScrewInstance()
	{
		_screwInstance.transform.SetLocalPositionAndRotation(GetScrewPosition(), Quaternion.identity);
		_drillScrewRotator.Add(_screwInstance.transform);
		_instantiator.AddComponent<CapsuleCollider>(_screwInstance).radius = _drillScrewBuilderSpec.DrillRadius;
	}

	private Vector3 GetScrewPosition()
	{
		Vector3 vector = _blockObject.Orientation.TransformInWorldSpace(_drillScrewBuilderSpec.AnchorPosition);
		Vector3 coordinates = _blockObject.Blocks.Pivot(_blockObject.Coordinates, _blockObject.Orientation);
		Vector3 position = vector + CoordinateSystem.GridToWorld(coordinates);
		return _parent.transform.InverseTransformPoint(position);
	}

	private int GetDistanceToBottomOfMap()
	{
		return Mathf.CeilToInt(_parent.transform.TransformPoint(_drillScrewBuilderSpec.AnchorPosition).y) + 1;
	}
}
