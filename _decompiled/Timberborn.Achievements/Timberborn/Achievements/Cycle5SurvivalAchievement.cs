using Timberborn.GameCycleSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class Cycle5SurvivalAchievement : CycleSurvivalAchievement
{
	public Cycle5SurvivalAchievement(EventBus eventBus, GameCycleService gameCycleService)
		: base(eventBus, gameCycleService, 5)
	{
	}
}
