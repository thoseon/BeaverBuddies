using Timberborn.EntitySystem;

namespace Timberborn.GameDistricts;

public class FinishedBuildingRegisteredEventArgs
{
	public EntityComponent Building { get; }

	public FinishedBuildingRegisteredEventArgs(EntityComponent building)
	{
		Building = building;
	}
}
