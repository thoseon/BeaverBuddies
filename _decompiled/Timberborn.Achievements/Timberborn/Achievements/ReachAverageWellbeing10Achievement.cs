using Timberborn.Wellbeing;

namespace Timberborn.Achievements;

internal class ReachAverageWellbeing10Achievement : ReachAverageWellbeingAchievement
{
	public ReachAverageWellbeing10Achievement(WellbeingService wellbeingService)
		: base(wellbeingService, 10)
	{
	}
}
