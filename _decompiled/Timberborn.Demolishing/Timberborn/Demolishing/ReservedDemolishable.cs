namespace Timberborn.Demolishing;

public class ReservedDemolishable
{
	public Demolishable Demolishable { get; }

	public bool ForceDemolish { get; }

	public bool CanBeDemolished
	{
		get
		{
			if (!Demolishable.IsMarked)
			{
				return ForceDemolish;
			}
			return true;
		}
	}

	public ReservedDemolishable(Demolishable demolishable, bool forceDemolish)
	{
		Demolishable = demolishable;
		ForceDemolish = forceDemolish;
	}
}
