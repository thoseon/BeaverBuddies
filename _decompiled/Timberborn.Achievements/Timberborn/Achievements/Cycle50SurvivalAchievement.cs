using Timberborn.GameCycleSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class Cycle50SurvivalAchievement : CycleSurvivalAchievement
{
	public Cycle50SurvivalAchievement(EventBus eventBus, GameCycleService gameCycleService)
		: base(eventBus, gameCycleService, 50)
	{
	}
}
