namespace Timberborn.InventorySystem;

public readonly struct DisallowedGoodsChangedEventArgs
{
	public string GoodId { get; }

	public DisallowedGoodsChangedEventArgs(string goodId)
	{
		GoodId = goodId;
	}
}
