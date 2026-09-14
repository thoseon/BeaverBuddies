using Timberborn.Beavers;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class ReachBeaverPopulation500Achievement : ReachBeaverPopulationAchievement
{
	public ReachBeaverPopulation500Achievement(BeaverPopulation beaverPopulation, EventBus eventBus)
		: base(beaverPopulation, eventBus, 500)
	{
	}
}
