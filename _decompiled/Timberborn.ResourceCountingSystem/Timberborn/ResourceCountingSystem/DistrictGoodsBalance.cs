using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.ResourceCountingSystem;

public class DistrictGoodsBalance : BaseComponent, IAwakableComponent, IPersistentEntity
{
	private static readonly ComponentKey DistrictGoodsBalanceKey = new ComponentKey("DistrictGoodsBalance");

	private static readonly PropertyKey<GoodRegistry> ProductionRegistryKey = new PropertyKey<GoodRegistry>("ProductionRegistry");

	private static readonly PropertyKey<GoodRegistry> ConsumptionRegistryKey = new PropertyKey<GoodRegistry>("ConsumptionRegistry");

	private readonly GoodRegistryValueSerializer _goodRegistryValueSerializer;

	private GoodRegistry _productionRegistry = new GoodRegistry();

	private GoodRegistry _consumptionRegistry = new GoodRegistry();

	public DistrictGoodsBalance(GoodRegistryValueSerializer goodRegistryValueSerializer)
	{
		_goodRegistryValueSerializer = goodRegistryValueSerializer;
	}

	public void Awake()
	{
		DistrictInventoryRegistry component = GetComponent<DistrictInventoryRegistry>();
		component.InventoryRegistered += OnInventoryRegistered;
		component.InventoryUnregistered += OnInventoryUnregistered;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(DistrictGoodsBalanceKey);
		component.Set(ProductionRegistryKey, _productionRegistry, _goodRegistryValueSerializer);
		component.Set(ConsumptionRegistryKey, _consumptionRegistry, _goodRegistryValueSerializer);
	}

	[BackwardCompatible(2026, 5, 7, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(DistrictGoodsBalanceKey, out var objectLoader))
		{
			_productionRegistry = objectLoader.Get(ProductionRegistryKey, _goodRegistryValueSerializer);
			_consumptionRegistry = objectLoader.Get(ConsumptionRegistryKey, _goodRegistryValueSerializer);
		}
	}

	public int GetProduction(string goodId)
	{
		return _productionRegistry.Amount(goodId);
	}

	public int GetConsumption(string goodId)
	{
		return _consumptionRegistry.Amount(goodId);
	}

	public void Flush()
	{
		_productionRegistry.Clear();
		_consumptionRegistry.Clear();
	}

	private void OnInventoryRegistered(object sender, Inventory inventory)
	{
		inventory.InventoryStockChanged += OnInventoryStockChanged;
	}

	private void OnInventoryUnregistered(object sender, Inventory inventory)
	{
		inventory.InventoryStockChanged -= OnInventoryStockChanged;
	}

	private void OnInventoryStockChanged(object sender, InventoryStockChangedEventArgs e)
	{
		switch (e.StockChangeType)
		{
		case StockChangeType.Produced:
			_productionRegistry.Add(e.GoodAmount);
			break;
		case StockChangeType.Consumed:
			_consumptionRegistry.Add(new GoodAmount(e.GoodAmount.GoodId, -e.GoodAmount.Amount));
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case StockChangeType.None:
		case StockChangeType.Imported:
		case StockChangeType.Exported:
			break;
		}
	}
}
