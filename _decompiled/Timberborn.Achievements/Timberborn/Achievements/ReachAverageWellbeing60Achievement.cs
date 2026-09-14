using Timberborn.Wellbeing;

namespace Timberborn.Achievements;

internal class ReachAverageWellbeing60Achievement : ReachAverageWellbeingAchievement
{
	public ReachAverageWellbeing60Achievement(WellbeingService wellbeingService)
		: base(wellbeingService, 60)
	{
	}
}
