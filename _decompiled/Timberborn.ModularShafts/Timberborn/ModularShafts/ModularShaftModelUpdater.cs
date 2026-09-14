using System;
using Bindito.Unity;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Coordinates;
using Timberborn.MechanicalSystem;
using Timberborn.MechanicalSystemUI;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.ModularShafts;

internal class ModularShaftModelUpdater : BaseComponent, IAwakableComponent, IMechanicalModelUpdater, IModelUpdater
{
	private static readonly Vector3 ModelOffset = new Vector3(0.5f, 0f, 0.5f);

	private readonly IInstantiator _instantiator;

	private readonly ModularShaftModelService _modularShaftModelService;

	private readonly MaterialColorer _materialColorer;

	private BlockObject _blockObject;

	private HighlightableObject _highlightableObject;

	private ModularShaftCover _modularShaftCover;

	private ModularShaftVariantFinder _modularShaftVariantFinder;

	private IBlockObjectModel _blockObjectModel;

	private MechanicalNode _mechanicalNode;

	private Transform _parent;

	private ShaftVariant _currentShaftVariant;

	private GameObject _modelInstance;

	public event EventHandler ModelUpdated;

	public ModularShaftModelUpdater(IInstantiator instantiator, ModularShaftModelService modularShaftModelService, MaterialColorer materialColorer)
	{
		_instantiator = instantiator;
		_modularShaftModelService = modularShaftModelService;
		_materialColorer = materialColorer;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_highlightableObject = GetComponent<HighlightableObject>();
		_modularShaftCover = GetComponent<ModularShaftCover>();
		_modularShaftVariantFinder = GetComponent<ModularShaftVariantFinder>();
		_blockObjectModel = GetComponent<IBlockObjectModel>();
		_mechanicalNode = GetComponent<MechanicalNode>();
		_mechanicalNode.AddedToGraph += delegate
		{
			UpdateModel();
		};
		_parent = GetComponent<BuildingModel>().FinishedModel.transform;
	}

	public void UpdateModel()
	{
		ShaftVariant shaftVariant = _modularShaftVariantFinder.FindBestVariant();
		if (!_modelInstance || shaftVariant != _currentShaftVariant)
		{
			_currentShaftVariant = shaftVariant;
			UpdateModelInstance();
		}
	}

	private void UpdateModelInstance()
	{
		if ((bool)_modelInstance)
		{
			RemoveModelInstance();
		}
		SpawnModelInstance();
		UpdateModelVisuals();
		ModelUpdated?.Invoke(this, EventArgs.Empty);
	}

	private void RemoveModelInstance()
	{
		_modelInstance.SetActive(value: false);
		_modelInstance.transform.SetParent(null, worldPositionStays: false);
		UnityEngine.Object.Destroy(_modelInstance);
		_modelInstance = null;
	}

	private void SpawnModelInstance()
	{
		GameObject value;
		Orientation orientation;
		GameObject gameObject;
		Orientation orientation2;
		if (!_modularShaftCover)
		{
			_modularShaftModelService.GetModel(_currentShaftVariant).Deconstruct(out value, out orientation);
			gameObject = value;
			orientation2 = orientation;
		}
		else
		{
			_modularShaftModelService.GetStackableModel(_currentShaftVariant).Deconstruct(out value, out orientation);
			gameObject = value;
			orientation2 = orientation;
		}
		_modelInstance = _instantiator.Instantiate(gameObject, _parent);
		_modelInstance.transform.SetLocalPositionAndRotation(ModelOffset, orientation2.ToWorldSpaceRotation());
		_modelInstance.name = gameObject.name + " @ " + orientation2;
		_modelInstance.SetActive(value: true);
	}

	private void UpdateModelVisuals()
	{
		if ((bool)_modularShaftCover)
		{
			_modularShaftCover.UpdateModel();
		}
		_highlightableObject.RefreshHighlight();
		if (_blockObject.IsUnfinished)
		{
			_materialColorer.EnableGrayscale(_modelInstance);
		}
		_blockObjectModel.UpdateModelVisibility();
	}
}
