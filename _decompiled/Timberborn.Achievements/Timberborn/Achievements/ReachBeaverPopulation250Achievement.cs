using Timberborn.Beavers;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class ReachBeaverPopulation250Achievement : ReachBeaverPopulationAchievement
{
	public ReachBeaverPopulation250Achievement(BeaverPopulation beaverPopulation, EventBus eventBus)
		: base(beaverPopulation, eventBus, 250)
	{
	}
}
