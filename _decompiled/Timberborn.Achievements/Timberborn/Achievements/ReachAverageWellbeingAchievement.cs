using Timberborn.AchievementSystem;
using Timberborn.TickSystem;
using Timberborn.Wellbeing;

namespace Timberborn.Achievements;

internal abstract class ReachAverageWellbeingAchievement : Achievement, ITickableSingleton
{
	private readonly WellbeingService _wellbeingService;

	private readonly int _threshold;

	public override string Id => $"REACH_{_threshold}_AVERAGE_WELLBEING";

	protected ReachAverageWellbeingAchievement(WellbeingService wellbeingService, int threshold)
	{
		_wellbeingService = wellbeingService;
		_threshold = threshold;
	}

	public void Tick()
	{
		if (base.IsEnabled && _wellbeingService.AverageGlobalWellbeing >= _threshold)
		{
			Unlock();
		}
	}
}
