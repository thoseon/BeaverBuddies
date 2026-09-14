using Timberborn.GameCycleSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class Cycle20SurvivalAchievement : CycleSurvivalAchievement
{
	public Cycle20SurvivalAchievement(EventBus eventBus, GameCycleService gameCycleService)
		: base(eventBus, gameCycleService, 20)
	{
	}
}
