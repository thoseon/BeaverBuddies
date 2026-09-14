using System.Collections.Generic;
using System.Linq;
using Timberborn.Goods;

namespace Timberborn.InventorySystem;

public class InventoryInitializer
{
	private readonly IGoodService _goodService;

	private readonly Inventory _inventory;

	private readonly int _capacity;

	private readonly string _componentName;

	private readonly List<StorableGoodAmount> _storableGoodAmounts = new List<StorableGoodAmount>();

	private bool _publicInput;

	private bool _publicOutput;

	private bool _ignorableCapacity;

	private IGoodDisallower _goodDisallower;

	public InventoryInitializer(IGoodService goodService, Inventory inventory, int capacity, string componentName)
	{
		_goodService = goodService;
		_inventory = inventory;
		_capacity = capacity;
		_componentName = componentName;
	}

	public void AddAllowedGood(StorableGoodAmount good)
	{
		_storableGoodAmounts.Add(good);
	}

	public void AddAllowedGoods(IEnumerable<StorableGoodAmount> goods)
	{
		_storableGoodAmounts.AddRange(goods);
	}

	public void AddAllowedGoodType(string goodType)
	{
		IEnumerable<StorableGood> source = GetGoods(goodType).Select(StorableGood.CreateGiveableAndTakeable);
		_storableGoodAmounts.AddRange(source.Select((StorableGood good) => new StorableGoodAmount(good, _capacity)));
	}

	public void HasPublicInput()
	{
		_publicInput = true;
	}

	public void HasPublicOutput()
	{
		_publicOutput = true;
	}

	public void SetIgnorableCapacity()
	{
		_ignorableCapacity = true;
	}

	public void AddGoodDisallower(IGoodDisallower goodDisallower)
	{
		_goodDisallower = goodDisallower;
	}

	public void Initialize()
	{
		IGoodDisallower goodDisallower = _goodDisallower ?? new NullGoodDisallower();
		_inventory.Initialize(_componentName, _capacity, _storableGoodAmounts, _publicInput, _publicOutput, _ignorableCapacity, goodDisallower);
		if (goodDisallower is IInitializableGoodDisallower initializableGoodDisallower)
		{
			initializableGoodDisallower.Initialize(_inventory);
		}
		AddToInventories();
	}

	private IEnumerable<string> GetGoods(string goodType)
	{
		return _goodService.Goods.Where((string good) => _goodService.GetGood(good).GoodType == goodType);
	}

	private void AddToInventories()
	{
		_inventory.GetComponent<Inventories>().AddInventory(_inventory);
	}
}
