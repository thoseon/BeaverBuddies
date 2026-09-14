using Timberborn.GameFactionSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class ActivateWonderFolktailsAchievement : ActivateWonderAchievement
{
	public ActivateWonderFolktailsAchievement(EventBus eventBus, FactionService factionService)
		: base(eventBus, factionService, AchievementHelper.Folktails)
	{
	}
}
