using Timberborn.BlockObjectModelSystem;
using Timberborn.LevelVisibilitySystem;

namespace Timberborn.ModelHiding;

internal class UncoveredModelHider
{
	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly HidableModels _hidableModels;

	public UncoveredModelHider(ILevelVisibilityService levelVisibilityService, HidableModels hidableModels)
	{
		_levelVisibilityService = levelVisibilityService;
		_hidableModels = hidableModels;
	}

	public void UpdateVisibility(int minLevel, int maxLevel)
	{
		int maxVisibleLevel = _levelVisibilityService.MaxVisibleLevel;
		for (int i = minLevel; i <= maxLevel; i++)
		{
			foreach (BlockObjectModelController item in _hidableModels.ModelsAt(i))
			{
				if (item.HasUncoveredModel)
				{
					if (item.BlockObject.GetTopLevel() > maxVisibleLevel)
					{
						item.ShowUncoveredModel();
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
		if (model.HasUncoveredModel)
		{
			int maxVisibleLevel = _levelVisibilityService.MaxVisibleLevel;
			if (model.BlockObject.GetTopLevel() > maxVisibleLevel)
			{
				model.ShowUncoveredModel();
			}
		}
	}
}
