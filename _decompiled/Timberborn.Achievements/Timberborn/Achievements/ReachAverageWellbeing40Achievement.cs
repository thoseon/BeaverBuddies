using Timberborn.Wellbeing;

namespace Timberborn.Achievements;

internal class ReachAverageWellbeing40Achievement : ReachAverageWellbeingAchievement
{
	public ReachAverageWellbeing40Achievement(WellbeingService wellbeingService)
		: base(wellbeingService, 40)
	{
	}
}
