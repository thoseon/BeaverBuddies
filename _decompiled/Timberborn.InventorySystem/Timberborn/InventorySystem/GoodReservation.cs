using Timberborn.Goods;

namespace Timberborn.InventorySystem;

public readonly struct GoodReservation
{
	public Inventory Inventory { get; }

	public GoodAmount GoodAmount { get; }

	public bool FixedAmount { get; }

	public bool ConsumeGood { get; }

	public GoodReservation(Inventory inventory, GoodAmount goodAmount, bool fixedAmount, bool consumeGood)
	{
		Inventory = inventory;
		GoodAmount = goodAmount;
		FixedAmount = fixedAmount;
		ConsumeGood = consumeGood;
	}
}
