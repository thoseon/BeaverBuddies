using System.Collections.Generic;
using Timberborn.Goods;
using Timberborn.InventorySystem;

namespace Timberborn.ResourceCountingSystem;

internal class ProcessedGoodCounter
{
	private readonly Dictionary<string, int> _processedStock = new Dictionary<string, int>();

	private readonly Dictionary<string, int> _awaitingProcessStock = new Dictionary<string, int>();

	private readonly List<IGoodProcessor> _goodProcessors = new List<IGoodProcessor>();

	public void UpdateStock()
	{
		_processedStock.Clear();
		_awaitingProcessStock.Clear();
		foreach (IGoodProcessor goodProcessor in _goodProcessors)
		{
			CountGoodProcessor(goodProcessor);
		}
	}

	public int GetProcessedStock(string goodId)
	{
		return _processedStock.GetValueOrDefault(goodId);
	}

	public int GetInputStock(string goodId)
	{
		return _awaitingProcessStock.GetValueOrDefault(goodId);
	}

	public void Add(IGoodProcessor goodProcessor)
	{
		_goodProcessors.Add(goodProcessor);
	}

	public void Remove(IGoodProcessor goodProcessor)
	{
		_goodProcessors.Remove(goodProcessor);
	}

	private void CountGoodProcessor(IGoodProcessor goodProcessor)
	{
		foreach (GoodAmount processedGood in goodProcessor.ProcessedGoods)
		{
			if (_processedStock.ContainsKey(processedGood.GoodId))
			{
				_processedStock[processedGood.GoodId] += processedGood.Amount;
			}
			else
			{
				_processedStock[processedGood.GoodId] = processedGood.Amount;
			}
		}
		Inventory inventory = goodProcessor.Inventory;
		if (!inventory)
		{
			return;
		}
		foreach (string inputGood in inventory.InputGoods)
		{
			if (inventory.Takes(inputGood))
			{
				if (_awaitingProcessStock.ContainsKey(inputGood))
				{
					_awaitingProcessStock[inputGood] += inventory.AmountInStock(inputGood);
				}
				else
				{
					_awaitingProcessStock.Add(inputGood, inventory.AmountInStock(inputGood));
				}
			}
		}
	}
}
