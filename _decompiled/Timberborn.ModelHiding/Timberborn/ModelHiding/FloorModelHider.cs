using System.Collections.Generic;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.ConstructionMode;
using Timberborn.LevelVisibilitySystem;
using UnityEngine;

namespace Timberborn.ModelHiding;

internal class FloorModelHider
{
	private static readonly List<BlockObjectModelController> EmptyModelsToUnblock = new List<BlockObjectModelController>();

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly IBlockService _blockService;

	private readonly ConstructionModeService _constructionModeService;

	private readonly HidableModels _hidableModels;

	public FloorModelHider(ILevelVisibilityService levelVisibilityService, IBlockService blockService, ConstructionModeService constructionModeService, HidableModels hidableModels)
	{
		_levelVisibilityService = levelVisibilityService;
		_blockService = blockService;
		_constructionModeService = constructionModeService;
		_hidableModels = hidableModels;
	}

	public void UpdateVisibility(int minLevel, int maxLevel, ICollection<BlockObjectModelController> modelsToUnblock)
	{
		for (int i = minLevel; i <= maxLevel; i++)
		{
			foreach (BlockObjectModelController item in _hidableModels.ModelsAt(i))
			{
				if (IsValidFloor(item))
				{
					if (CanShowUncoveredFloor(item, modelsToUnblock))
					{
						item.ShowUncoveredModel();
						modelsToUnblock.Add(item);
					}
					else
					{
						item.HideUncoveredModel();
					}
				}
			}
		}
	}

	public void ShowModelIfPossible(BlockObjectModelController model)
	{
		if (IsValidFloor(model) && CanShowUncoveredFloor(model, EmptyModelsToUnblock))
		{
			model.UnblockModel();
			model.ShowUncoveredModel();
		}
	}

	private bool IsValidFloor(BlockObjectModelController model)
	{
		if (model.HasUncoveredModel && _levelVisibilityService.BlockIsVisible(model.BlockObject.CoordinatesAtBaseZ.Below()))
		{
			return model.BlockObject.IsFloor();
		}
		return false;
	}

	private bool CanShowUncoveredFloor(BlockObjectModelController model, ICollection<BlockObjectModelController> modelsToUnblock)
	{
		if (model.BlockObject.IsUnfinished && !_constructionModeService.InConstructionMode)
		{
			return true;
		}
		if (model.BlockObject.GetTopLevel() > _levelVisibilityService.MaxVisibleLevel)
		{
			return HasFullyShownBlockObjectBelow(model, modelsToUnblock);
		}
		return false;
	}

	private bool HasFullyShownBlockObjectBelow(BlockObjectModelController model, ICollection<BlockObjectModelController> modelsToUnblock)
	{
		Vector3Int coordinates = model.BlockObject.CoordinatesAtBaseZ.Below();
		foreach (BlockObject item in _blockService.GetObjectsAt(coordinates))
		{
			if (!item.IsFloor() && !IsFullyShown(item, modelsToUnblock))
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsFullyShown(BlockObject blockObject, ICollection<BlockObjectModelController> modelsToUnblock)
	{
		BlockObjectModelController component = blockObject.GetComponent<BlockObjectModelController>();
		if ((bool)component && (component.IsUncoveredModelShown || !component.IsAnyModelShown))
		{
			if (!component.ShouldShowUncoveredModel)
			{
				return modelsToUnblock.Contains(component);
			}
			return false;
		}
		return true;
	}
}
