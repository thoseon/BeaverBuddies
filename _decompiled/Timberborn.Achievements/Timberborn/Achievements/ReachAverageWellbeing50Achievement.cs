using Timberborn.Wellbeing;

namespace Timberborn.Achievements;

internal class ReachAverageWellbeing50Achievement : ReachAverageWellbeingAchievement
{
	public ReachAverageWellbeing50Achievement(WellbeingService wellbeingService)
		: base(wellbeingService, 50)
	{
	}
}
