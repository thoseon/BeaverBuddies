using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.InventorySystem;

namespace Timberborn.ResourceCountingSystem;

internal class StockCounter
{
	private readonly Dictionary<string, int> _outputStock = new Dictionary<string, int>();

	private readonly Dictionary<string, int> _inputOutputStock = new Dictionary<string, int>();

	public void UpdateStock(ReadOnlyHashSet<Inventory> inventories)
	{
		_outputStock.Clear();
		_inputOutputStock.Clear();
		foreach (Inventory item in inventories)
		{
			CountInventoryStock(item);
		}
	}

	public int GetInputOutputStock(string goodId)
	{
		return _inputOutputStock.GetValueOrDefault(goodId, 0);
	}

	public int GetOutputStock(string goodId)
	{
		return _outputStock.GetValueOrDefault(goodId, 0);
	}

	private void CountInventoryStock(Inventory inventory)
	{
		foreach (GoodAmount item in inventory.Stock)
		{
			CountStock(item, inventory);
		}
	}

	private void CountStock(GoodAmount good, Inventory inventory)
	{
		if (inventory.Gives(good.GoodId))
		{
			Dictionary<string, int> stock = (inventory.PublicInput ? _inputOutputStock : _outputStock);
			CountStock(good, stock);
		}
	}

	private static void CountStock(GoodAmount good, IDictionary<string, int> stock)
	{
		string goodId = good.GoodId;
		if (stock.ContainsKey(goodId))
		{
			stock[goodId] += good.Amount;
		}
		else
		{
			stock[goodId] = good.Amount;
		}
	}
}
