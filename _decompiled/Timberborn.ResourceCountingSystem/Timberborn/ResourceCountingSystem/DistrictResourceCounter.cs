using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Carrying;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;

namespace Timberborn.ResourceCountingSystem;

public class DistrictResourceCounter : TickableComponent, IAwakableComponent, IPostLoadableEntity
{
	private readonly StockCounter _stockCounter = new StockCounter();

	private readonly CapacityCounter _capacityCounter = new CapacityCounter();

	private readonly ProcessedGoodCounter _processedGoodCounter = new ProcessedGoodCounter();

	private readonly GoodRegistry _availableCarriedGoods = new GoodRegistry();

	private readonly GoodRegistry _reservedCarriedGoods = new GoodRegistry();

	private readonly List<GoodCarrier> _goodCarriers = new List<GoodCarrier>();

	private DistrictInventoryRegistry _districtInventoryRegistry;

	public void Awake()
	{
		_districtInventoryRegistry = GetComponent<DistrictInventoryRegistry>();
	}

	public void PostLoadEntity()
	{
		UpdateCounters();
	}

	public override void Tick()
	{
		UpdateCounters();
	}

	[OnEvent]
	public void OnNewGameInitialized(NewGameInitializedEvent newGameInitializedEvent)
	{
		UpdateCounters();
	}

	public ResourceCount GetResourceCount(string goodId)
	{
		return ResourceCount.Create(_stockCounter.GetInputOutputStock(goodId), _stockCounter.GetOutputStock(goodId), _capacityCounter.GetInputOutputCapacity(goodId), _capacityCounter.GetOutputCapacity(goodId), _availableCarriedGoods.Amount(goodId), _reservedCarriedGoods.Amount(goodId), _processedGoodCounter.GetProcessedStock(goodId), _processedGoodCounter.GetInputStock(goodId));
	}

	public void Add(GoodCarrier goodCarrier)
	{
		_goodCarriers.Add(goodCarrier);
	}

	public void Remove(GoodCarrier goodCarrier)
	{
		_goodCarriers.Remove(goodCarrier);
	}

	public void Add(IGoodProcessor goodProcessor)
	{
		_processedGoodCounter.Add(goodProcessor);
	}

	public void Remove(IGoodProcessor goodProcessor)
	{
		_processedGoodCounter.Remove(goodProcessor);
	}

	public void UpdateCounters()
	{
		ReadOnlyHashSet<Inventory> inventories = _districtInventoryRegistry.Inventories;
		_stockCounter.UpdateStock(inventories);
		_capacityCounter.UpdateCapacity(inventories);
		_processedGoodCounter.UpdateStock();
		UpdateCarriedGoods();
	}

	private void UpdateCarriedGoods()
	{
		_availableCarriedGoods.Clear();
		_reservedCarriedGoods.Clear();
		foreach (GoodCarrier goodCarrier in _goodCarriers)
		{
			if (goodCarrier.IsCarrying)
			{
				if (goodCarrier.CarriedGood.Type == CarriedGoodType.Available)
				{
					_availableCarriedGoods.Add(goodCarrier.CarriedGood.GoodAmount);
				}
				else if (goodCarrier.CarriedGood.Type == CarriedGoodType.Unavailable)
				{
					_reservedCarriedGoods.Add(goodCarrier.CarriedGood.GoodAmount);
				}
			}
		}
	}
}
