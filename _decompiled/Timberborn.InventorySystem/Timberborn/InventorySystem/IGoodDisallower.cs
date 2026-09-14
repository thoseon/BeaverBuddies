using System;

namespace Timberborn.InventorySystem;

public interface IGoodDisallower
{
	event EventHandler<DisallowedGoodsChangedEventArgs> DisallowedGoodsChanged;

	int AllowedAmount(string goodId);
}
