using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;

namespace Timberborn.BlockObjectModelSystem;

public class BlockObjectModelController : BaseComponent, IAwakableComponent, IPostInitializableEntity
{
	private IBlockObjectModel _blockObjectModel;

	private IInfiniteUndergroundModel _infiniteUndergroundModel;

	private readonly List<IModelUpdater> _modelUpdaters = new List<IModelUpdater>();

	private bool _modelBlocked;

	private bool _modelBlockageIgnored;

	public bool IsAnyModelShown { get; private set; }

	public bool IsFinishedModelShown { get; private set; }

	public bool IsUncoveredModelShown { get; private set; }

	public bool ShouldShowUncoveredModel { get; private set; }

	public bool ShouldShowUndergroundModel { get; private set; }

	public BlockObject BlockObject { get; private set; }

	public int UndergroundModelZOffset { get; private set; }

	public bool HasUndergroundModel => _blockObjectModel.HasUndergroundModel;

	public bool ModelBlocked
	{
		get
		{
			if (_modelBlocked)
			{
				return !_modelBlockageIgnored;
			}
			return false;
		}
	}

	public bool HasUncoveredModel => _blockObjectModel.HasUncoveredModel;

	public int UndergroundBaseZ
	{
		get
		{
			if (_infiniteUndergroundModel == null)
			{
				return Math.Max(0, BlockObject.CoordinatesAtBaseZ.z - _blockObjectModel.UndergroundModelDepth);
			}
			return 0;
		}
	}

	public event EventHandler ModelsUpdated;

	public void Awake()
	{
		BlockObject = GetComponent<BlockObject>();
		_blockObjectModel = GetComponent<IBlockObjectModel>();
		_infiniteUndergroundModel = GetComponent<IInfiniteUndergroundModel>();
		GetComponents(_modelUpdaters);
	}

	public void PostInitializeEntity()
	{
		UpdateAll();
	}

	public void UpdateModel()
	{
		foreach (IModelUpdater modelUpdater in _modelUpdaters)
		{
			modelUpdater.UpdateModel();
		}
	}

	public void UpdateAll()
	{
		UpdateModel();
		_blockObjectModel.UpdateModelVisibility();
	}

	public void IgnoreModelBlockade()
	{
		_modelBlockageIgnored = true;
		UpdateModelVisibility();
	}

	public void UnignoreModelBlockade()
	{
		_modelBlockageIgnored = false;
		UpdateModelVisibility();
	}

	public void BlockModel()
	{
		_modelBlocked = true;
		UpdateModelVisibility();
	}

	public void UnblockModel()
	{
		_modelBlocked = false;
		UpdateModelVisibility();
	}

	public void ShowUncoveredModel()
	{
		ShouldShowUncoveredModel = true;
		UpdateModelVisibility();
	}

	public void HideUncoveredModel()
	{
		ShouldShowUncoveredModel = false;
		UpdateModelVisibility();
	}

	public void ShowUndergroundModel()
	{
		ShouldShowUndergroundModel = true;
		UpdateModelVisibility();
	}

	public void HideUndergroundModel()
	{
		ShouldShowUndergroundModel = false;
		SetUndergroundModelZOffset(0);
	}

	public void SetUndergroundModelZOffset(int zOffset)
	{
		UndergroundModelZOffset = zOffset;
		UpdateModelVisibility();
	}

	public void SetModelState(bool isModelShown, bool isFinishedModelShown, bool isUncoveredModelShown)
	{
		IsAnyModelShown = isModelShown;
		IsFinishedModelShown = isFinishedModelShown;
		IsUncoveredModelShown = isUncoveredModelShown;
		ModelsUpdated?.Invoke(this, EventArgs.Empty);
	}

	private void UpdateModelVisibility()
	{
		_blockObjectModel.UpdateModelVisibility();
	}
}
