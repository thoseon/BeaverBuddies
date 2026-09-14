using Timberborn.Beavers;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class ReachBeaverPopulation100Achievement : ReachBeaverPopulationAchievement
{
	public ReachBeaverPopulation100Achievement(BeaverPopulation beaverPopulation, EventBus eventBus)
		: base(beaverPopulation, eventBus, 100)
	{
	}
}
