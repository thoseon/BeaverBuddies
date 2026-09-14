namespace Timberborn.Demolishing;

public class DemolishableMarkedEvent
{
	public Demolishable Demolishable { get; }

	public DemolishableMarkedEvent(Demolishable demolishable)
	{
		Demolishable = demolishable;
	}
}
