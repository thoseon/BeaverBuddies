using System;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.MergeableObjects;

internal class MergeableObjectModel : BaseComponent, IAwakableComponent, IModelUpdater
{
	private readonly IBlockService _blockService;

	private readonly PreviewBlockService _previewBlockService;

	private TemplateSpec _templateSpec;

	private BlockObject _blockObject;

	private MergeableObjectModelSpec _mergeableObjectModelSpec;

	private GameObject _currentModel;

	private Orientation _currentModelOrientation;

	private readonly NeighboredValues4<GameObject> _models = new NeighboredValues4<GameObject>();

	public MergeableObjectModel(IBlockService blockService, PreviewBlockService previewBlockService)
	{
		_blockService = blockService;
		_previewBlockService = previewBlockService;
	}

	public void Awake()
	{
		_templateSpec = GetComponent<TemplateSpec>();
		_blockObject = GetComponent<BlockObject>();
		_mergeableObjectModelSpec = GetComponent<MergeableObjectModelSpec>();
		ValidateSize();
		InitializeModels();
		SetMatchingModel(down: true, left: false, up: true, right: false);
	}

	public void UpdateModel()
	{
		Vector3Int origin = _blockObject.PositionedBlocks.GetOccupiedCoordinates().First();
		SetMatchingModel(IsMatchingInDirection(origin, Direction2D.Down), IsMatchingInDirection(origin, Direction2D.Left), IsMatchingInDirection(origin, Direction2D.Up), IsMatchingInDirection(origin, Direction2D.Right));
	}

	private void ValidateSize()
	{
		if (_blockObject.Blocks.Size != Vector3Int.one)
		{
			throw new InvalidOperationException(_templateSpec.TemplateName + " validation failed. MergeableObjectModel is only compatible with 1x1x1-sized buildings");
		}
	}

	private void InitializeModels()
	{
		AddModel("0000", down: false, left: false, up: false, right: false);
		AddModel("0001", down: false, left: true, up: false, right: false);
		AddModel("1010", down: false, left: true, up: false, right: true);
		AddModel("0011", down: false, left: false, up: true, right: true);
		AddModel("0111", down: false, left: true, up: true, right: true);
		AddModel("1111", down: true, left: true, up: true, right: true);
	}

	private void AddModel(string variant, bool down, bool left, bool up, bool right)
	{
		string childName = _mergeableObjectModelSpec.ModelNamePrefix + variant;
		GameObject gameObject = base.GameObject.FindChild(childName);
		gameObject.SetActive(value: false);
		_models.AddVariants(gameObject, down, left, up, right);
	}

	private void SetMatchingModel(bool down, bool left, bool up, bool right)
	{
		var (model, orientation2) = _models.GetMatch(down, left, up, right);
		SetCurrentModel(model, orientation2);
	}

	private void SetCurrentModel(GameObject model, Orientation orientation)
	{
		if (_currentModel != model || _currentModelOrientation != orientation)
		{
			if ((bool)_currentModel)
			{
				_currentModel.SetActive(value: false);
			}
			Vector3 localPosition = CoordinateSystem.GridToWorld(orientation.ToPivotOffset());
			Quaternion localRotation = orientation.ToWorldSpaceRotation();
			model.transform.SetLocalPositionAndRotation(localPosition, localRotation);
			_currentModel = model;
			_currentModelOrientation = orientation;
			_currentModel.SetActive(value: true);
		}
	}

	private bool IsMatchingInDirection(Vector3Int origin, Direction2D direction2D)
	{
		Direction2D direction2D2 = _blockObject.Orientation.Transform(direction2D);
		Vector3Int vector3Int = origin + direction2D2.ToOffset();
		MergeableObjectModel bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<MergeableObjectModel>(vector3Int);
		MergeableObjectModel bottomObjectComponentAt2 = _previewBlockService.GetBottomObjectComponentAt<MergeableObjectModel>(vector3Int);
		if (!IsMatchingType(bottomObjectComponentAt) && !IsMatchingType(bottomObjectComponentAt2))
		{
			return IsEnforced(vector3Int);
		}
		return true;
	}

	private bool IsMatchingType(MergeableObjectModel otherModel)
	{
		if ((bool)otherModel)
		{
			return otherModel._templateSpec.TemplateName == _templateSpec.TemplateName;
		}
		return false;
	}

	private bool IsEnforced(Vector3Int target)
	{
		MergeableObjectModelEnforcerSpec bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<MergeableObjectModelEnforcerSpec>(target);
		MergeableObjectModelEnforcerSpec bottomObjectComponentAt2 = _previewBlockService.GetBottomObjectComponentAt<MergeableObjectModelEnforcerSpec>(target);
		if (!(bottomObjectComponentAt != null))
		{
			return bottomObjectComponentAt2 != null;
		}
		return true;
	}
}
