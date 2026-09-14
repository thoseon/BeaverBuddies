using Timberborn.Goods;

namespace Timberborn.InventorySystem;

public readonly struct InventoryReservationChangedEventArgs
{
	public GoodAmount GoodAmount { get; }

	public InventoryReservationChangedEventArgs(GoodAmount goodAmount)
	{
		GoodAmount = goodAmount;
	}
}
