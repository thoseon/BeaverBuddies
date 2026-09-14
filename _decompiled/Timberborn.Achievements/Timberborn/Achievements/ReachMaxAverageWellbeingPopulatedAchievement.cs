using Timberborn.AchievementSystem;
using Timberborn.Beavers;
using Timberborn.TickSystem;
using Timberborn.Wellbeing;

namespace Timberborn.Achievements;

internal class ReachMaxAverageWellbeingPopulatedAchievement : Achievement, ITickableSingleton
{
	private static readonly int RequiredPopulation = 100;

	private readonly WellbeingService _wellbeingService;

	private readonly WellbeingLimitService _wellbeingLimitService;

	private readonly BeaverPopulation _beaverPopulation;

	public override string Id => "REACH_MAX_AVERAGE_WELLBEING_POPULATED";

	public ReachMaxAverageWellbeingPopulatedAchievement(WellbeingService wellbeingService, WellbeingLimitService wellbeingLimitService, BeaverPopulation beaverPopulation)
	{
		_wellbeingService = wellbeingService;
		_wellbeingLimitService = wellbeingLimitService;
		_beaverPopulation = beaverPopulation;
	}

	public void Tick()
	{
		if (base.IsEnabled && _beaverPopulation.NumberOfBeavers >= RequiredPopulation && _wellbeingService.AverageGlobalWellbeing >= _wellbeingLimitService.MaxBeaverWellbeing)
		{
			Unlock();
		}
	}
}
