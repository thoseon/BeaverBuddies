using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.SlotSystem;
using UnityEngine;

namespace Timberborn.ConstructionSites;

public class ConstructionSiteProgressVisualizer : BaseComponent, IAwakableComponent, IUnfinishedStateListener, IFinishedStateListener
{
	private ConstructionSite _constructionSite;

	private BlockObject _blockObject;

	private SlotManager _slotManager;

	private BuildingModel _buildingModel;

	private ConstructionSiteProgressVisualizerSpec _constructionSiteProgressVisualizerSpec;

	private readonly List<GameObject> _stages = new List<GameObject>();

	private int _slotsIndex = -1;

	public bool ShouldShowProgress => _constructionSite.WasStarted;

	private ImmutableArray<float> ProgressThresholds => _constructionSiteProgressVisualizerSpec.ProgressThresholds;

	public event EventHandler StageChanged;

	public void Awake()
	{
		_constructionSite = GetComponent<ConstructionSite>();
		_blockObject = GetComponent<BlockObject>();
		_slotManager = GetComponent<SlotManager>();
		_buildingModel = GetComponent<BuildingModel>();
		_constructionSiteProgressVisualizerSpec = GetComponent<ConstructionSiteProgressVisualizerSpec>();
		InitializeStages();
		UpdateVisualization();
	}

	public void OnEnterUnfinishedState()
	{
		_constructionSite.OnConstructionSiteProgressed += OnConstructionSiteProgressed;
		UpdateVisualization();
	}

	public void OnExitUnfinishedState()
	{
		_constructionSite.OnConstructionSiteProgressed -= OnConstructionSiteProgressed;
	}

	public void OnEnterFinishedState()
	{
		UpdateVisualization();
	}

	public void OnExitFinishedState()
	{
	}

	private void InitializeStages()
	{
		if ((bool)_buildingModel.UnfinishedModel)
		{
			foreach (Transform item in _buildingModel.UnfinishedModel.transform)
			{
				_stages.Add(item.gameObject);
			}
			if (ProgressThresholds.Length != _stages.Count - 1)
			{
				throw new Exception($"Number of thresholds ({ProgressThresholds.Length}) is not equal to number of " + $"stages minus ConstructionBase ({_stages.Count - 1}) in BuildingModel " + "of " + base.Name);
			}
			return;
		}
		throw new Exception("Unfinished model not found in BuildingModel of " + base.Name);
	}

	private void OnConstructionSiteProgressed(object sender, EventArgs e)
	{
		UpdateVisualization();
	}

	private void UpdateVisualization()
	{
		HideAll();
		if (!_blockObject.IsFinished && _blockObject.Positioned)
		{
			int stageIndex = GetStageIndex();
			_stages[stageIndex].SetActive(value: true);
			StageChanged?.Invoke(this, EventArgs.Empty);
			ReassignAllSlots(stageIndex);
		}
	}

	private void HideAll()
	{
		foreach (GameObject stage in _stages)
		{
			stage.SetActive(value: false);
		}
	}

	private int GetStageIndex()
	{
		if (ShouldShowProgress)
		{
			for (int num = _stages.Count - 1; num >= 1; num--)
			{
				if (_constructionSite.BuildTimeProgress >= ProgressThresholds[num - 1])
				{
					return num;
				}
			}
		}
		return 0;
	}

	private void ReassignAllSlots(int stageIndex)
	{
		if (_slotsIndex != stageIndex && (bool)_slotManager)
		{
			_slotsIndex = stageIndex;
			_slotManager.ReassignAllSlots();
		}
	}
}
