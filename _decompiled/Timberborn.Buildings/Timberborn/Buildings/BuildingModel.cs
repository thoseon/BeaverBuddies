using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Buildings;

public class BuildingModel : BaseComponent, IAwakableComponent, IInitializablePreview, IBlockObjectModel, IFinishedStateListener, IUnfinishedStateListener
{
	private BlockObject _blockObject;

	private BlockObjectModelController _blockObjectModelController;

	private BuildingModelSpec _buildingModelSpec;

	private bool _showFinishedModel;

	private bool _blockUnfinishedModel;

	private GameObject _undergroundModel;

	public GameObject FinishedModel { get; private set; }

	public GameObject UnfinishedModel { get; private set; }

	public GameObject FinishedUncoveredModel { get; private set; }

	public int UndergroundModelDepth { get; private set; }

	public bool HasUnfinishedModel => UnfinishedModel;

	public bool HasUncoveredModel => FinishedUncoveredModel;

	public bool HasUndergroundModel => _undergroundModel;

	public bool UnfinishedConstructionModeModel => _buildingModelSpec.UnfinishedConstructionModeModel;

	public bool IsUnfinishedModelBlocked => _blockUnfinishedModel;

	private bool ForceUnfinishedModel
	{
		get
		{
			if (UnfinishedConstructionModeModel && _blockObject.IsUnfinished)
			{
				return !_blockUnfinishedModel;
			}
			return false;
		}
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectModelController = GetComponent<BlockObjectModelController>();
		_buildingModelSpec = GetComponent<BuildingModelSpec>();
		UndergroundModelDepth = _buildingModelSpec.UndergroundModelDepth;
		FinishedModel = base.GameObject.FindChildIfNameNotEmpty(_buildingModelSpec.FinishedModelName);
		UnfinishedModel = base.GameObject.FindChildIfNameNotEmpty(_buildingModelSpec.UnfinishedModelName);
		FinishedUncoveredModel = base.GameObject.FindChildIfNameNotEmpty(_buildingModelSpec.FinishedUncoveredModelName);
		_undergroundModel = base.GameObject.FindChildIfNameNotEmpty(_buildingModelSpec.UndergroundModelName);
	}

	public void InitializePreview()
	{
		ShowFinishedModel();
	}

	public void OnEnterFinishedState()
	{
		ShowFinishedModel();
	}

	public void OnExitFinishedState()
	{
	}

	public void OnEnterUnfinishedState()
	{
		ShowUnfinishedModel();
	}

	public void OnExitUnfinishedState()
	{
	}

	public void ShowFinishedModel()
	{
		_showFinishedModel = true;
		UpdateModelVisibility();
	}

	public void ShowUnfinishedModel()
	{
		_showFinishedModel = false;
		UpdateModelVisibility();
	}

	public void BlockUnfinishedModel()
	{
		_blockUnfinishedModel = true;
		UpdateModelVisibility();
	}

	public void UnblockUnfinishedModel()
	{
		_blockUnfinishedModel = false;
		UpdateModelVisibility();
	}

	public void UpdateModelVisibility()
	{
		if (_blockObjectModelController.ModelBlocked)
		{
			ToggleModelVisibility(showFinished: false, !_blockObject.IsPreview && _showFinishedModel, showFinishedUncovered: false, showUnfinished: false, showUndergroundModel: false);
		}
		else if (ForceUnfinishedModel)
		{
			ToggleModelVisibility(showFinished: false, showFinishedShadows: false, showFinishedUncovered: false, showUnfinished: true, showUndergroundModel: false);
		}
		else if (_showFinishedModel)
		{
			bool shouldShowUncoveredModel = _blockObjectModelController.ShouldShowUncoveredModel;
			bool shouldShowUndergroundModel = _blockObjectModelController.ShouldShowUndergroundModel;
			ToggleModelVisibility(!(shouldShowUncoveredModel | shouldShowUndergroundModel), showFinishedShadows: true, shouldShowUncoveredModel, showUnfinished: false, shouldShowUndergroundModel);
		}
		else
		{
			bool shouldShowUndergroundModel2 = _blockObjectModelController.ShouldShowUndergroundModel;
			ToggleModelVisibility(showFinished: false, showFinishedShadows: false, showFinishedUncovered: false, !_blockUnfinishedModel && !shouldShowUndergroundModel2, shouldShowUndergroundModel2);
		}
	}

	private void ToggleModelVisibility(bool showFinished, bool showFinishedShadows, bool showFinishedUncovered, bool showUnfinished, bool showUndergroundModel)
	{
		FinishedModel.ToggleModelVisibility(showFinished, showFinishedShadows);
		FinishedUncoveredModel.ToggleModelVisibility(showFinishedUncovered, showShadows: false);
		UnfinishedModel.ToggleModelVisibility(showUnfinished, showUnfinished);
		_undergroundModel.ToggleModelVisibility(showUndergroundModel, showShadows: false);
		if (showUndergroundModel)
		{
			Vector3 localPosition = _undergroundModel.transform.localPosition;
			_undergroundModel.transform.localPosition = new Vector3(localPosition.x, _blockObjectModelController.UndergroundModelZOffset, localPosition.z);
		}
		_blockObjectModelController.SetModelState(showFinished | showUnfinished, showFinished | showFinishedUncovered, showFinishedUncovered);
	}
}
