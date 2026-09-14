using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.LevelVisibilitySystem;
using UnityEngine;

namespace Timberborn.Buildings;

internal class UncoveredModelSwitcher : BaseComponent, IAwakableComponent, IInitializableEntity, IPostPlacementChangeListener, IDeletableEntity, IPrePreviewShownListener
{
	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly List<GameObject> _fullModels = new List<GameObject>();

	private readonly List<GameObject> _uncoveredModels = new List<GameObject>();

	private BlockObject _blockObject;

	public UncoveredModelSwitcher(ILevelVisibilityService levelVisibilityService)
	{
		_levelVisibilityService = levelVisibilityService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		CollectModels();
	}

	public void InitializeEntity()
	{
		_levelVisibilityService.MaxVisibleLevelChanged += OnMaxVisibleLevelChanged;
		UpdateVisibility();
	}

	public void OnPostPlacementChanged()
	{
		UpdateVisibility();
	}

	public void DeleteEntity()
	{
		_levelVisibilityService.MaxVisibleLevelChanged -= OnMaxVisibleLevelChanged;
	}

	public void OnPrePreviewShown()
	{
		UpdateVisibility();
	}

	private void CollectModels()
	{
		UncoveredModelSwitcherSpec component = GetComponent<UncoveredModelSwitcherSpec>();
		BuildingModel component2 = GetComponent<BuildingModel>();
		CollectModelsFromChildren(component2.UnfinishedModel, component);
		CollectModelsFromChildren(component2.FinishedModel, component);
	}

	private void OnMaxVisibleLevelChanged(object sender, int level)
	{
		UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		int maxVisibleLevel = _levelVisibilityService.MaxVisibleLevel;
		int z = _blockObject.CoordinatesAtBaseZ.z;
		int num = z + _blockObject.Blocks.Size.z - 1;
		bool flag = maxVisibleLevel >= z && maxVisibleLevel <= num;
		foreach (GameObject fullModel in _fullModels)
		{
			fullModel.SetActive(!flag);
		}
		foreach (GameObject uncoveredModel in _uncoveredModels)
		{
			uncoveredModel.SetActive(flag);
		}
	}

	private void CollectModelsFromChildren(GameObject model, UncoveredModelSwitcherSpec uncoverModelSwitcherSpec)
	{
		foreach (GameObject allChild in model.GetAllChildren())
		{
			if (allChild.name == uncoverModelSwitcherSpec.FullModelName)
			{
				_fullModels.Add(allChild.gameObject);
			}
			else if (allChild.name == uncoverModelSwitcherSpec.UncoveredModelName)
			{
				_uncoveredModels.Add(allChild.gameObject);
			}
		}
	}
}
