using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;

namespace Timberborn.Goods;

public class GoodRegistry
{
	private readonly List<GoodAmount> _registry = new List<GoodAmount>();

	public int TotalAmount { get; private set; }

	public ReadOnlyList<GoodAmount> Goods => _registry.AsReadOnlyList();

	public void Clear()
	{
		_registry.Clear();
		TotalAmount = 0;
	}

	public void Add(GoodAmount good)
	{
		AssertValueIsPositive(good);
		Modify(good.GoodId, good.Amount);
	}

	public void Subtract(GoodAmount good)
	{
		AssertValueIsPositive(good);
		Modify(good.GoodId, -good.Amount);
	}

	public int Amount(string goodId)
	{
		for (int i = 0; i < _registry.Count; i++)
		{
			GoodAmount goodAmount = _registry[i];
			if (goodAmount.GoodId == goodId)
			{
				return goodAmount.Amount;
			}
		}
		return 0;
	}

	public override string ToString()
	{
		return Goods.CollectionToString("Goods");
	}

	private void Modify(string goodId, int amount)
	{
		int num = GetAmountAndRemove(goodId) + amount;
		if (num != 0)
		{
			_registry.Add(new GoodAmount(goodId, num));
		}
		TotalAmount += amount;
		ValidateTotalAmount();
	}

	private static void AssertValueIsPositive(GoodAmount goodAmount)
	{
		Asserts.ValueIsInRange(goodAmount.Amount, 0, int.MaxValue, "GoodAmount");
	}

	private int GetAmountAndRemove(string goodId)
	{
		for (int i = 0; i < _registry.Count; i++)
		{
			if (_registry[i].GoodId == goodId)
			{
				int amount = _registry[i].Amount;
				_registry.RemoveAt(i);
				return amount;
			}
		}
		return 0;
	}

	private void ValidateTotalAmount()
	{
		if (TotalAmount < 0)
		{
			int num = _registry.Sum((GoodAmount good) => good.Amount);
			throw new InvalidOperationException(string.Format("{0} {1} is negative ", "TotalAmount", TotalAmount) + $"and sum of goods is {num}");
		}
	}
}
