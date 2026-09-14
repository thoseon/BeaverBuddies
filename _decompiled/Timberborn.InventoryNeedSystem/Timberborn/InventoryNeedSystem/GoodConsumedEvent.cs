namespace Timberborn.InventoryNeedSystem;

public struct GoodConsumedEvent
{
	public string GoodId { get; }

	public GoodConsumedEvent(string goodId)
	{
		GoodId = goodId;
	}
}
