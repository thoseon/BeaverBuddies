using Timberborn.EntitySystem;

namespace Timberborn.GameDistricts;

public class FinishedBuildingInstantUnregisteredEventArgs
{
	public EntityComponent Building { get; }

	public FinishedBuildingInstantUnregisteredEventArgs(EntityComponent building)
	{
		Building = building;
	}
}
