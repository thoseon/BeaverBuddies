using Timberborn.BaseComponentSystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.TickSystem;

namespace Timberborn.DistributionSystem;

internal class DistrictCrossingAutoExporter : TickableComponent, IAwakableComponent
{
	private DistrictCrossing _districtCrossing;

	private DistrictCrossingInventory _districtCrossingInventory;

	private Inventory Inventory => _districtCrossingInventory.Inventory;

	public void Awake()
	{
		_districtCrossing = GetComponent<DistrictCrossing>();
		_districtCrossingInventory = GetComponent<DistrictCrossingInventory>();
	}

	public override void Tick()
	{
		if (_districtCrossing.CanExport && Inventory.IsUnblocked)
		{
			TryExport();
		}
	}

	private void TryExport()
	{
		for (int num = Inventory.Stock.Count - 1; num >= 0; num--)
		{
			GoodAmount goodAmount = Inventory.Stock[num];
			if (Inventory.UnreservedAmountInStock(goodAmount.GoodId) > 0)
			{
				TryExportStock(goodAmount);
			}
		}
	}

	private void TryExportStock(GoodAmount goodAmount)
	{
		if (_districtCrossing.TryGetLinkedDistrictDistributableGood(goodAmount.GoodId, out var distributableGood))
		{
			DistributableGood myDistributableGood = _districtCrossing.GetMyDistributableGood(goodAmount.GoodId);
			if (_districtCrossing.CanExportGood(myDistributableGood, distributableGood))
			{
				ExportStock(myDistributableGood, distributableGood);
			}
		}
	}

	private void ExportStock(DistributableGood myDistributableGood, DistributableGood linkedDistributableGood)
	{
		int amountToExport = _districtCrossing.GetAmountToExport(myDistributableGood, linkedDistributableGood);
		if (amountToExport > 0)
		{
			_districtCrossingInventory.TransferStock(myDistributableGood.GoodId, amountToExport);
		}
	}
}
