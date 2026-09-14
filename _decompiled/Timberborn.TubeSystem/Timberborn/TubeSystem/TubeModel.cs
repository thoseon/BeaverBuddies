using System;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.TubeSystem;

internal class TubeModel : BaseComponent, IAwakableComponent, IModelUpdater
{
	private readonly TubeConnectionService _tubeConnectionService;

	private BuildingSpec _buildingSpec;

	private TubeModelSpec _tubeModelSpec;

	private BlockObject _blockObject;

	private GameObject _currentModel;

	private Orientation _currentModelOrientation;

	private readonly NeighboredValues6<GameObject> _models = new NeighboredValues6<GameObject>();

	public TubeModel(TubeConnectionService tubeConnectionService)
	{
		_tubeConnectionService = tubeConnectionService;
	}

	public void Awake()
	{
		_buildingSpec = GetComponent<BuildingSpec>();
		_tubeModelSpec = GetComponent<TubeModelSpec>();
		_blockObject = GetComponent<BlockObject>();
		ValidateSize();
		InitializeModels();
		UpdateModel(down: false, left: false, up: false, right: false, top: false, bottom: false);
	}

	public void UpdateModel()
	{
		Vector3Int origin = GetOrigin();
		UpdateModel(CanConnectInDirection(origin, Direction3D.Down), CanConnectInDirection(origin, Direction3D.Left), CanConnectInDirection(origin, Direction3D.Up), CanConnectInDirection(origin, Direction3D.Right), CanConnectInDirection(origin, Direction3D.Top), CanConnectInDirection(origin, Direction3D.Bottom));
	}

	private void ValidateSize()
	{
		if (GetComponent<BlockObjectSpec>().Size != Vector3Int.one)
		{
			throw new InvalidOperationException(_buildingSpec.GetSpec<TemplateSpec>().TemplateName + " validation failed. TubeModel is only compatible with 1x1x1-sized buildings");
		}
	}

	private void InitializeModels()
	{
		foreach (GameObject allChild in base.GameObject.GetAllChildren())
		{
			AddModelIfValid(allChild);
		}
	}

	private void AddModelIfValid(GameObject model)
	{
		if (model.name.StartsWith(_tubeModelSpec.ModelPrefix))
		{
			model.SetActive(value: false);
			string name = model.name;
			int length = _tubeModelSpec.ModelPrefix.Length;
			string text = name.Substring(length, name.Length - length);
			_models.AddVariants(model, text[0] == '1', text[1] == '1', text[2] == '1', text[3] == '1', text[4] == '1', text[5] == '1');
		}
	}

	private void UpdateModel(bool down, bool left, bool up, bool right, bool top, bool bottom)
	{
		var (model, orientation2) = _models.GetMatch(down, left, up, right, top, bottom);
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

	private Vector3Int GetOrigin()
	{
		foreach (Block item in from block in _blockObject.PositionedBlocks.GetOccupiedBlocks()
			orderby block.Coordinates.z descending
			select block)
		{
			if (item.Occupation.Intersects(BlockOccupations.Path))
			{
				return item.Coordinates;
			}
		}
		throw new InvalidOperationException(_buildingSpec.GetSpec<TemplateSpec>().TemplateName + " validation failed. TubeModel should have at least one block intersecting with Path, whose neighbors are checked when selecting a fitting model.");
	}

	private bool CanConnectInDirection(Vector3Int origin, Direction3D direction)
	{
		Direction3D direction3D = direction.RotateHorizontally(_blockObject.Orientation);
		return _tubeConnectionService.CanConnectInDirection(origin, direction3D);
	}
}
