using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.InventorySystem;

namespace Timberborn.ResourceCountingSystem;

internal class CapacityCounter
{
	private readonly Dictionary<string, int> _outputCapacity = new Dictionary<string, int>();

	private readonly Dictionary<string, int> _inputOutputCapacity = new Dictionary<string, int>();

	private readonly List<GoodAmount> _temporaryCapacityCache = new List<GoodAmount>();

	public void UpdateCapacity(ReadOnlyHashSet<Inventory> inventories)
	{
		_outputCapacity.Clear();
		_inputOutputCapacity.Clear();
		foreach (Inventory item in inventories)
		{
			CountInventoryCapacity(item);
		}
	}

	public int GetInputOutputCapacity(string goodId)
	{
		return _inputOutputCapacity.GetValueOrDefault(goodId, 0);
	}

	public int GetOutputCapacity(string goodId)
	{
		return _outputCapacity.GetValueOrDefault(goodId, 0);
	}

	private void CountInventoryCapacity(Inventory inventory)
	{
		inventory.GetCapacity(_temporaryCapacityCache);
		foreach (GoodAmount item in _temporaryCapacityCache)
		{
			CountCapacity(item, inventory);
		}
		_temporaryCapacityCache.Clear();
	}

	private void CountCapacity(GoodAmount good, Inventory inventory)
	{
		if (inventory.Gives(good.GoodId))
		{
			Dictionary<string, int> capacity = (inventory.PublicInput ? _inputOutputCapacity : _outputCapacity);
			CountCapacity(good, capacity);
		}
	}

	private static void CountCapacity(GoodAmount good, IDictionary<string, int> capacity)
	{
		string goodId = good.GoodId;
		if (capacity.ContainsKey(goodId))
		{
			capacity[goodId] += good.Amount;
		}
		else
		{
			capacity[goodId] = good.Amount;
		}
	}
}
