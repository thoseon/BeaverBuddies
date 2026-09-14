using Timberborn.GameFactionSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class ActivateWonderIronTeethAchievement : ActivateWonderAchievement
{
	public ActivateWonderIronTeethAchievement(EventBus eventBus, FactionService factionService)
		: base(eventBus, factionService, AchievementHelper.IronTeeth)
	{
	}
}
