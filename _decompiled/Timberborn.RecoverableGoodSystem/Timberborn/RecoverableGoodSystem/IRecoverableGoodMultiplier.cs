using Timberborn.InventorySystem;

namespace Timberborn.RecoverableGoodSystem;

public interface IRecoverableGoodMultiplier
{
	float GetMultiplierForInventory(Inventory inventory);
}
