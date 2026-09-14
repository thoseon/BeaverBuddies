using Timberborn.GameCycleSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class Cycle10SurvivalAchievement : CycleSurvivalAchievement
{
	public Cycle10SurvivalAchievement(EventBus eventBus, GameCycleService gameCycleService)
		: base(eventBus, gameCycleService, 10)
	{
	}
}
