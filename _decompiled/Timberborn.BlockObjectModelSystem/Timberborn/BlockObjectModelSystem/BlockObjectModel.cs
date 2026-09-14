using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.BlockObjectModelSystem;

public class BlockObjectModel : BaseComponent, IAwakableComponent, IBlockObjectModel, IInitializablePreview
{
	private BlockObjectModelController _blockObjectModelController;

	private GameObject _uncoveredModel;

	private GameObject _undergroundModel;

	private bool _fullModelPermanentlyHidden;

	public GameObject FullModel { get; private set; }

	public int UndergroundModelDepth { get; private set; }

	public bool HasUndergroundModel => _undergroundModel;

	public bool HasUncoveredModel => _uncoveredModel;

	public bool UnfinishedConstructionModeModel => false;

	public void Awake()
	{
		_blockObjectModelController = GetComponent<BlockObjectModelController>();
		BlockObjectModelSpec component = GetComponent<BlockObjectModelSpec>();
		UndergroundModelDepth = component.UndergroundModelDepth;
		FullModel = base.GameObject.FindChildIfNameNotEmpty(component.FullModelName);
		_uncoveredModel = base.GameObject.FindChildIfNameNotEmpty(component.UncoveredModelName);
		_undergroundModel = base.GameObject.FindChildIfNameNotEmpty(component.UndergroundModelName);
	}

	public void InitializePreview()
	{
		UpdateModelVisibility();
	}

	public void UpdateModelVisibility()
	{
		bool modelBlocked = _blockObjectModelController.ModelBlocked;
		bool shouldShowUncoveredModel = _blockObjectModelController.ShouldShowUncoveredModel;
		bool shouldShowUndergroundModel = _blockObjectModelController.ShouldShowUndergroundModel;
		bool flag = !modelBlocked && !shouldShowUncoveredModel && !shouldShowUndergroundModel && !_fullModelPermanentlyHidden;
		bool flag2 = !modelBlocked & shouldShowUncoveredModel;
		bool flag3 = !modelBlocked & shouldShowUndergroundModel;
		if (flag3)
		{
			Vector3 localPosition = _undergroundModel.transform.localPosition;
			_undergroundModel.transform.localPosition = new Vector3(localPosition.x, _blockObjectModelController.UndergroundModelZOffset, localPosition.z);
		}
		FullModel.ToggleModelVisibility(flag, showShadows: true);
		_uncoveredModel.ToggleModelVisibility(flag2, showShadows: false);
		_undergroundModel.ToggleModelVisibility(flag3, showShadows: false);
		_blockObjectModelController.SetModelState(flag | flag2, isFinishedModelShown: true, flag2);
	}

	public void HideFullModelPermanently()
	{
		_fullModelPermanentlyHidden = true;
		UpdateModelVisibility();
	}

	public void UnhideFullModelPermanently()
	{
		_fullModelPermanentlyHidden = false;
		UpdateModelVisibility();
	}
}
