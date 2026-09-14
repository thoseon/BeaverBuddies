namespace Timberborn.Demolishing;

public class DemolishableUnmarkedEvent
{
	public Demolishable Demolishable { get; }

	public DemolishableUnmarkedEvent(Demolishable demolishable)
	{
		Demolishable = demolishable;
	}
}
