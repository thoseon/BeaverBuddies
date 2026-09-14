using Timberborn.AchievementSystem;
using Timberborn.TickSystem;
using Timberborn.Wellbeing;

namespace Timberborn.Achievements;

internal class ReachMaxAverageWellbeingAchievement : Achievement, ITickableSingleton
{
	private readonly WellbeingService _wellbeingService;

	private readonly WellbeingLimitService _wellbeingLimitService;

	public override string Id => "REACH_MAX_AVERAGE_WELLBEING";

	public ReachMaxAverageWellbeingAchievement(WellbeingService wellbeingService, WellbeingLimitService wellbeingLimitService)
	{
		_wellbeingService = wellbeingService;
		_wellbeingLimitService = wellbeingLimitService;
	}

	public void Tick()
	{
		if (base.IsEnabled && _wellbeingService.AverageGlobalWellbeing >= _wellbeingLimitService.MaxBeaverWellbeing)
		{
			Unlock();
		}
	}
}
