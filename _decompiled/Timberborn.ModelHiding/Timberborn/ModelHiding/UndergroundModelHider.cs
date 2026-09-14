using System.Collections.Generic;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.ModelHiding;

internal class UndergroundModelHider
{
	private readonly ITerrainService _terrainService;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly HidableModels _hidableModels;

	public UndergroundModelHider(ITerrainService terrainService, ILevelVisibilityService levelVisibilityService, HidableModels hidableModels)
	{
		_terrainService = terrainService;
		_levelVisibilityService = levelVisibilityService;
		_hidableModels = hidableModels;
	}

	public void UpdateVisibility(int minLevel, int maxLevel, ICollection<BlockObjectModelController> modelsToUnblock)
	{
		for (int i = minLevel; i <= maxLevel; i++)
		{
			foreach (BlockObjectModelController item in _hidableModels.ModelsAt(i))
			{
				if (item.HasUndergroundModel)
				{
					if (CanShowUndergroundModel(item))
					{
						ShowUndergroundModel(item);
						modelsToUnblock.Add(item);
					}
					else
					{
						item.HideUndergroundModel();
					}
				}
			}
		}
	}

	public void ShowModelIfPossible(BlockObjectModelController model)
	{
		if (model.HasUndergroundModel && CanShowUndergroundModel(model))
		{
			model.UnblockModel();
			ShowUndergroundModel(model);
		}
	}

	private bool CanShowUndergroundModel(BlockObjectModelController model)
	{
		BlockObject blockObject = model.BlockObject;
		int maxVisibleLevel = _levelVisibilityService.MaxVisibleLevel;
		if (model.UndergroundBaseZ <= maxVisibleLevel && blockObject.CoordinatesAtBaseZ.z > maxVisibleLevel)
		{
			foreach (Vector3Int foundationCoordinate in blockObject.PositionedBlocks.GetFoundationCoordinates())
			{
				if (!_terrainService.Underground(new Vector3Int(foundationCoordinate.x, foundationCoordinate.y, maxVisibleLevel)))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private void ShowUndergroundModel(BlockObjectModelController model)
	{
		model.ShowUndergroundModel();
		model.SetUndergroundModelZOffset(_levelVisibilityService.MaxVisibleLevel - model.BlockObject.CoordinatesAtBaseZ.z + 1);
	}
}
