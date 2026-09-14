using System.Collections.Generic;

namespace Timberborn.Wellbeing;

public interface IWellbeingTierService
{
	IEnumerable<string> GetTierableBonuses(WellbeingTracker wellbeingTracker);

	bool TryGetTierBonus(WellbeingTracker wellbeingTracker, string bonusId, int wellbeing, out WellbeingTierBonus tierBonus);

	bool TryGetNextTierBonus(WellbeingTracker wellbeingTracker, string bonusId, int wellbeing, out WellbeingTierBonus nextTierBonus);
}
