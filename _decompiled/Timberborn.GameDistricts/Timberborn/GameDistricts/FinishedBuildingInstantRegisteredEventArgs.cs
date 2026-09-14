using Timberborn.EntitySystem;

namespace Timberborn.GameDistricts;

public class FinishedBuildingInstantRegisteredEventArgs
{
	public EntityComponent Building { get; }

	public FinishedBuildingInstantRegisteredEventArgs(EntityComponent building)
	{
		Building = building;
	}
}
