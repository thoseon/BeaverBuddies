using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.TickSystem;
using Timberborn.WorkSystem;

namespace Timberborn.Yielding;

public class InRangeYielderGoodAllower : TickableComponent, IAwakableComponent, IFinishedStateListener, IInitializableGoodDisallower, IGoodDisallower
{
	private InRangeYielders _inRangeYielders;

	private Workplace _workplace;

	private readonly HashSet<string> _allowedGoods = new HashSet<string>();

	private readonly HashSet<string> _yieldsCache = new HashSet<string>();

	private readonly List<string> _incomingGoods = new List<string>();

	private Inventory _inventory;

	public event EventHandler<DisallowedGoodsChangedEventArgs> DisallowedGoodsChanged;

	public void Awake()
	{
		_inRangeYielders = GetComponent<InRangeYielders>();
		_workplace = GetComponent<Workplace>();
	}

	public void Initialize(Inventory inventory)
	{
		_inventory = inventory;
	}

	public override void Tick()
	{
		for (int num = _incomingGoods.Count - 1; num >= 0; num--)
		{
			string good = _incomingGoods[num];
			if (_inventory.ReservedCapacity(good) == 0)
			{
				_incomingGoods.RemoveAt(num);
			}
		}
		if (_incomingGoods.Count == 0)
		{
			DisableComponent();
		}
	}

	public void OnEnterFinishedState()
	{
		_inRangeYielders.YieldersChanged += OnYieldersChanged;
		_inRangeYielders.YielderAdded += OnYielderAdded;
		_inventory.InventoryCapacityReservationChanged += OnInventoryCapacityReservationChanged;
		UpdateAllowedGoods();
	}

	public void OnExitFinishedState()
	{
		_inRangeYielders.YieldersChanged -= OnYieldersChanged;
		_inRangeYielders.YielderAdded -= OnYielderAdded;
		_inventory.InventoryCapacityReservationChanged -= OnInventoryCapacityReservationChanged;
	}

	public int AllowedAmount(string goodId)
	{
		if (!AllowsGood(goodId))
		{
			return 0;
		}
		return int.MaxValue;
	}

	private void OnYieldersChanged(object sender, EventArgs e)
	{
		UpdateAllowedGoods();
	}

	private void OnYielderAdded(object sender, Yielder yielder)
	{
		string id = yielder.YielderSpec.Yield.Id;
		if (_allowedGoods.Add(id))
		{
			DisallowedGoodsChanged?.Invoke(this, new DisallowedGoodsChangedEventArgs(id));
		}
	}

	private void OnInventoryCapacityReservationChanged(object sender, InventoryReservationChangedEventArgs e)
	{
		GoodAmount goodAmount = e.GoodAmount;
		if (goodAmount.Amount > 0 && !_incomingGoods.Contains(goodAmount.GoodId))
		{
			_incomingGoods.Add(goodAmount.GoodId);
			EnableComponent();
		}
	}

	private bool AllowsGood(string goodId)
	{
		if ((!_allowedGoods.Contains(goodId) || _workplace.NumberOfAssignedWorkers <= 0) && _inventory.AmountInStock(goodId) <= 0)
		{
			return _incomingGoods.Contains(goodId);
		}
		return true;
	}

	private void UpdateAllowedGoods()
	{
		_inRangeYielders.GetYields(_yieldsCache);
		foreach (StorableGoodAmount allowedGood in _inventory.AllowedGoods)
		{
			string goodId = allowedGood.StorableGood.GoodId;
			if (!TryAdd(goodId))
			{
				TryRemove(goodId);
			}
		}
		_yieldsCache.Clear();
	}

	private bool TryAdd(string goodId)
	{
		if (_yieldsCache.Contains(goodId) && !_allowedGoods.Contains(goodId))
		{
			Add(goodId);
			return true;
		}
		return false;
	}

	private void Add(string goodId)
	{
		_allowedGoods.Add(goodId);
		DisallowedGoodsChanged?.Invoke(this, new DisallowedGoodsChangedEventArgs(goodId));
	}

	private void TryRemove(string goodId)
	{
		if (_allowedGoods.Contains(goodId) && !_yieldsCache.Contains(goodId))
		{
			Remove(goodId);
		}
	}

	private void Remove(string goodId)
	{
		_allowedGoods.Remove(goodId);
		DisallowedGoodsChanged?.Invoke(this, new DisallowedGoodsChangedEventArgs(goodId));
	}
}
