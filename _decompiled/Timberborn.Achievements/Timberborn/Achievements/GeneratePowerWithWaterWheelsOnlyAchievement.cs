using Timberborn.MechanicalSystem;
using Timberborn.PowerGeneration;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class GeneratePowerWithWaterWheelsOnlyAchievement : GeneratePowerWithAchievement<WaterPoweredGenerator>
{
	public GeneratePowerWithWaterWheelsOnlyAchievement(MechanicalGraphRegistry mechanicalGraphRegistry, EventBus eventBus)
		: base(mechanicalGraphRegistry, eventBus, "GENERATE_POWER_WITH_WATER_WHEELS_ONLY", 10000)
	{
	}
}
