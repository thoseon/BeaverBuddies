using Timberborn.EntitySystem;

namespace Timberborn.GameDistricts;

public class FinishedBuildingUnregisteredEventArgs
{
	public EntityComponent Building { get; }

	public FinishedBuildingUnregisteredEventArgs(EntityComponent building)
	{
		Building = building;
	}
}
