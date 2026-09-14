using Timberborn.Goods;

namespace Timberborn.Yielding;

public class YieldReservationCompletedEventArgs
{
	public GoodAmount Yield { get; }

	public Yielder Yielder { get; }

	public YieldReservationCompletedEventArgs(GoodAmount yield, Yielder yielder)
	{
		Yield = yield;
		Yielder = yielder;
	}
}
