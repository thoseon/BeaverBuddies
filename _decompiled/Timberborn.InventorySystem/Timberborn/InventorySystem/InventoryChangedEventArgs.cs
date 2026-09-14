namespace Timberborn.InventorySystem;

public readonly struct InventoryChangedEventArgs
{
	public string GoodId { get; }

	public InventoryChangedEventArgs(string goodId)
	{
		GoodId = goodId;
	}
}
