namespace Timberborn.InventorySystem;

public interface IAmountProvider
{
	int UnreservedAmountInStock(string goodId);

	int UnreservedCapacity(string goodId);
}
