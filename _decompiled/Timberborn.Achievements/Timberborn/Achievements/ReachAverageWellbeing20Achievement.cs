using Timberborn.Wellbeing;

namespace Timberborn.Achievements;

internal class ReachAverageWellbeing20Achievement : ReachAverageWellbeingAchievement
{
	public ReachAverageWellbeing20Achievement(WellbeingService wellbeingService)
		: base(wellbeingService, 20)
	{
	}
}
