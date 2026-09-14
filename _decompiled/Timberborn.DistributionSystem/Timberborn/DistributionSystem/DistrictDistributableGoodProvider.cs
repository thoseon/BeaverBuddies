using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Emptying;
using Timberborn.InventorySystem;
using Timberborn.StockpilePrioritySystem;

namespace Timberborn.DistributionSystem;

public class DistrictDistributableGoodProvider : BaseComponent, IAwakableComponent
{
	private DistrictDistributionSetting _districtDistributionSetting;

	private DistributionInventoryRegistry _distributionInventoryRegistry;

	private readonly Dictionary<string, DistributableGood> _exportCache = new Dictionary<string, DistributableGood>();

	private readonly Dictionary<string, ImportableGood> _importCache = new Dictionary<string, ImportableGood>();

	public void Awake()
	{
		_districtDistributionSetting = GetComponent<DistrictDistributionSetting>();
		_distributionInventoryRegistry = GetComponent<DistributionInventoryRegistry>();
		_districtDistributionSetting.SettingChanged += delegate(object _, GoodDistributionSetting setting)
		{
			ClearCache(setting.GoodId);
		};
		_distributionInventoryRegistry.GoodStorageChanged += delegate(object _, string goodId)
		{
			ClearCache(goodId);
		};
	}

	public void GetDistributableGoodsForImport(List<DistributableGood> distributableGoods)
	{
		foreach (GoodDistributionSetting goodDistributionSetting in _districtDistributionSetting.GoodDistributionSettings)
		{
			if (TryGetDistributableGoodForImport(goodDistributionSetting.GoodId, out var distributableGood))
			{
				distributableGoods.Add(distributableGood);
			}
		}
		distributableGoods.Sort();
	}

	public bool TryGetDistributableGoodForImport(string goodId, out DistributableGood distributableGood)
	{
		ImportableGood importableGood = GetImportableGood(goodId);
		distributableGood = importableGood.DistributableGood;
		return importableGood.IsImportable;
	}

	public DistributableGood GetDistributableGoodForExport(string goodId)
	{
		if (_exportCache.TryGetValue(goodId, out var value))
		{
			return value;
		}
		GoodDistributionSetting goodDistributionSetting = _districtDistributionSetting.GetGoodDistributionSetting(goodId);
		return GetAndCacheExportDistributableGood(goodDistributionSetting);
	}

	public bool IsImportEnabled(string goodId)
	{
		ImportableGood importableGood = GetImportableGood(goodId);
		if (!importableGood.IsImportable)
		{
			return importableGood.HasCapacity;
		}
		return true;
	}

	public ImportOption GetGoodImportOption(string goodId)
	{
		return _districtDistributionSetting.GetGoodDistributionSetting(goodId).ImportOption;
	}

	private void ClearCache(string goodId)
	{
		_exportCache.Remove(goodId);
		_importCache.Remove(goodId);
	}

	private ImportableGood GetImportableGood(string goodId)
	{
		if (_importCache.TryGetValue(goodId, out var value))
		{
			return value;
		}
		GoodDistributionSetting goodDistributionSetting = _districtDistributionSetting.GetGoodDistributionSetting(goodId);
		value = (CanBeImported(goodDistributionSetting, out var hasCapacity) ? ImportableGood.CreateImportableWithCapacity(GetDistributableGood(goodDistributionSetting, withDistrictCrossingIncomingStock: true)) : ((!hasCapacity) ? ImportableGood.CreateNonImportable() : ImportableGood.CreateNonImportableWithCapacity()));
		_importCache.Add(goodId, value);
		return value;
	}

	private bool CanBeImported(GoodDistributionSetting goodDistributionSetting, out bool hasCapacity)
	{
		hasCapacity = goodDistributionSetting.ImportOption == ImportOption.Forced;
		if (goodDistributionSetting.ImportOption != ImportOption.Forced)
		{
			if (goodDistributionSetting.ImportOption == ImportOption.Auto)
			{
				return HasUnreservedCapacity(goodDistributionSetting, out hasCapacity);
			}
			return false;
		}
		return true;
	}

	private bool HasUnreservedCapacity(GoodDistributionSetting goodDistributionSetting, out bool hasCapacity)
	{
		hasCapacity = false;
		foreach (Inventory item in _distributionInventoryRegistry.CapacityInventories(goodDistributionSetting.GoodId))
		{
			if (item.IsUnblocked && GetInventoryCapacity(item, goodDistributionSetting.GoodId) > 0)
			{
				hasCapacity = true;
				if (item.HasUnreservedCapacity(goodDistributionSetting.GoodId))
				{
					return true;
				}
			}
		}
		return false;
	}

	private DistributableGood GetAndCacheExportDistributableGood(GoodDistributionSetting goodDistributionSetting)
	{
		DistributableGood distributableGood = GetDistributableGood(goodDistributionSetting, withDistrictCrossingIncomingStock: false);
		_exportCache.Add(goodDistributionSetting.GoodId, distributableGood);
		return distributableGood;
	}

	private DistributableGood GetDistributableGood(GoodDistributionSetting goodDistributionSetting, bool withDistrictCrossingIncomingStock)
	{
		int capacity = GetCapacity(goodDistributionSetting);
		return new DistributableGood(GetStock(goodDistributionSetting.GoodId, withDistrictCrossingIncomingStock), capacity, goodDistributionSetting);
	}

	private int GetCapacity(GoodDistributionSetting goodDistributionSetting)
	{
		string goodId = goodDistributionSetting.GoodId;
		int num = 0;
		foreach (Inventory item in _distributionInventoryRegistry.StoringInventories(goodId))
		{
			if (item.IsUnblocked)
			{
				num += GetInventoryCapacity(item, goodId);
			}
		}
		if (goodDistributionSetting.ImportOption == ImportOption.Forced || (num == 0 && HasTakingInventory(goodId)))
		{
			num += GetDistrictCrossingsCapacity(goodId);
		}
		return num;
	}

	private static int GetInventoryCapacity(Inventory inventory, string goodId)
	{
		Emptiable component = inventory.GetComponent<Emptiable>();
		if (component == null || !component.IsMarkedForEmptying)
		{
			GoodSupplier component2 = inventory.GetComponent<GoodSupplier>();
			if (component2 == null || !component2.IsSupplying)
			{
				return inventory.LimitedAmount(goodId);
			}
		}
		return 0;
	}

	private bool HasTakingInventory(string goodId)
	{
		foreach (Inventory item in _distributionInventoryRegistry.CapacityInventories(goodId))
		{
			if (item.IsUnblocked && GetInventoryCapacity(item, goodId) > 0)
			{
				return true;
			}
		}
		return false;
	}

	private int GetDistrictCrossingsCapacity(string goodId)
	{
		ReadOnlyHashSet<Inventory> districtCrossingInventories = _distributionInventoryRegistry.DistrictCrossingInventories;
		int num = 0;
		foreach (Inventory item in districtCrossingInventories)
		{
			num += item.LimitedAmount(goodId);
		}
		return num;
	}

	private int GetStock(string goodId, bool withDistrictCrossingIncomingStock)
	{
		int num = 0;
		foreach (Inventory item in _distributionInventoryRegistry.StockInventories(goodId))
		{
			num += GetInventoryStock(item, goodId, withDistrictCrossingIncomingStock);
		}
		return num;
	}

	private static int GetInventoryStock(Inventory inventory, string goodId, bool withDistrictCrossingIncomingStock)
	{
		int num = inventory.UnreservedAmountInStock(goodId);
		if (IsDistrictCrossingInventory(inventory))
		{
			if (withDistrictCrossingIncomingStock)
			{
				num += inventory.GetComponent<DistrictCrossingInventory>().IncomingStock(goodId);
			}
		}
		else
		{
			num += inventory.ReservedCapacity(goodId);
		}
		return num;
	}

	private static bool IsDistrictCrossingInventory(Inventory inventory)
	{
		return inventory.ComponentName == DistrictCrossingInventoryInitializer.InventoryComponentName;
	}
}
