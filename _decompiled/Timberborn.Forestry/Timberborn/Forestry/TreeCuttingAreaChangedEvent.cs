namespace Timberborn.Forestry;

public class TreeCuttingAreaChangedEvent
{
	public bool CoordinatesAdded { get; }

	public TreeCuttingAreaChangedEvent(bool coordinatesAdded = false)
	{
		CoordinatesAdded = coordinatesAdded;
	}
}
