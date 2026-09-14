using Timberborn.Buildings;

namespace Timberborn.ScienceSystem;

public class BuildingUnlockedEvent
{
	public BuildingSpec BuildingSpec { get; }

	public BuildingUnlockedEvent(BuildingSpec buildingSpec)
	{
		BuildingSpec = buildingSpec;
	}
}
