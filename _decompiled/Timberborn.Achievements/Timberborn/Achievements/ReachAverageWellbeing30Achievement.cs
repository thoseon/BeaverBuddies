using Timberborn.Wellbeing;

namespace Timberborn.Achievements;

internal class ReachAverageWellbeing30Achievement : ReachAverageWellbeingAchievement
{
	public ReachAverageWellbeing30Achievement(WellbeingService wellbeingService)
		: base(wellbeingService, 30)
	{
	}
}
