using Timberborn.Coordinates;

namespace Timberborn.GameStartup;

public class StartingBuildingPlacedEvent
{
	public Placement Placement { get; }

	public StartingBuildingPlacedEvent(Placement placement)
	{
		Placement = placement;
	}
}
