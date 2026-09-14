using Timberborn.AchievementSystem;

namespace Timberborn.Achievements;

internal abstract class PlantTreesAchievement : Achievement
{
	private readonly TreePlantingCounter _treePlantingCounter;

	private readonly int _threshold;

	public override string Id => $"PLANT_{_threshold}_TREES";

	protected PlantTreesAchievement(TreePlantingCounter treePlantingCounter, int threshold)
	{
		_treePlantingCounter = treePlantingCounter;
		_threshold = threshold;
	}

	protected override void EnableInternal()
	{
		_treePlantingCounter.CountChanged += OnCountChanged;
	}

	protected override void DisableInternal()
	{
		_treePlantingCounter.CountChanged -= OnCountChanged;
	}

	private void OnCountChanged(object sender, int plantedCount)
	{
		if (plantedCount >= _threshold)
		{
			Unlock();
		}
	}
}
