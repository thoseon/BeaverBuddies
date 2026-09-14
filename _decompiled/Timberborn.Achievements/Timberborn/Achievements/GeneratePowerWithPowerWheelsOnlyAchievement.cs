using Timberborn.MechanicalSystem;
using Timberborn.PowerGeneration;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class GeneratePowerWithPowerWheelsOnlyAchievement : GeneratePowerWithAchievement<WalkerPoweredGenerator>
{
	public GeneratePowerWithPowerWheelsOnlyAchievement(MechanicalGraphRegistry mechanicalGraphRegistry, EventBus eventBus)
		: base(mechanicalGraphRegistry, eventBus, "GENERATE_POWER_WITH_POWER_WHEELS_ONLY", 2000)
	{
	}
}
