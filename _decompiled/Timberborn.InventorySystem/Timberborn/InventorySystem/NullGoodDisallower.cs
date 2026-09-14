using System;

namespace Timberborn.InventorySystem;

public class NullGoodDisallower : IGoodDisallower
{
	public event EventHandler<DisallowedGoodsChangedEventArgs> DisallowedGoodsChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	public int AllowedAmount(string goodId)
	{
		return int.MaxValue;
	}
}
