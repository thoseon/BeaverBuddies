using Timberborn.MechanicalSystem;
using Timberborn.PowerGeneration;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class GeneratePowerWithWindTurbinesOnlyAchievement : GeneratePowerWithAchievement<WindPoweredGenerator>
{
	public GeneratePowerWithWindTurbinesOnlyAchievement(MechanicalGraphRegistry mechanicalGraphRegistry, EventBus eventBus)
		: base(mechanicalGraphRegistry, eventBus, "GENERATE_POWER_WITH_WIND_TURBINES_ONLY", 10000)
	{
	}
}
