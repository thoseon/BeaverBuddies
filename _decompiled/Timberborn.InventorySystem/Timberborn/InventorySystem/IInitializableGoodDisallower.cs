namespace Timberborn.InventorySystem;

public interface IInitializableGoodDisallower : IGoodDisallower
{
	void Initialize(Inventory inventory);
}
