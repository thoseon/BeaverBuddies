using Timberborn.Goods;

namespace Timberborn.InventorySystem;

public readonly struct InventoryStockChangedEventArgs
{
	public GoodAmount GoodAmount { get; }

	public StockChangeType StockChangeType { get; }

	public InventoryStockChangedEventArgs(GoodAmount goodAmount, StockChangeType stockChangeType)
	{
		GoodAmount = goodAmount;
		StockChangeType = stockChangeType;
	}
}
