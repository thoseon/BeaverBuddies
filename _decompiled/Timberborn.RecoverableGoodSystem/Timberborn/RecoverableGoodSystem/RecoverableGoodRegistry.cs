using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Goods;

namespace Timberborn.RecoverableGoodSystem;

public class RecoverableGoodRegistry
{
	private readonly List<GoodAmount> _goodAmounts = new List<GoodAmount>();

	public int TotalAmount { get; private set; }

	public ReadOnlyList<GoodAmount> GoodAmounts => _goodAmounts.AsReadOnlyList();

	public void Add(GoodAmount goodAmount)
	{
		AssertValueIsPositive(goodAmount);
		GoodAmount goodAmount2 = ClampGoodAmount(goodAmount);
		if (goodAmount2.Amount != 0)
		{
			AddInternal(goodAmount2);
		}
	}

	public void TakePercent(float percentage, ICollection<GoodAmount> takenGoods)
	{
		for (int num = _goodAmounts.Count - 1; num >= 0; num--)
		{
			GoodAmount goodAmount = _goodAmounts[num];
			int num2 = (int)Math.Round(percentage * (float)goodAmount.Amount);
			if (num2 > 0)
			{
				int num3 = goodAmount.Amount - num2;
				if (num3 > 0)
				{
					_goodAmounts[num] = new GoodAmount(goodAmount.GoodId, num3);
				}
				else
				{
					_goodAmounts.RemoveAt(num);
				}
				TotalAmount -= num2;
				takenGoods.Add(new GoodAmount(goodAmount.GoodId, num2));
			}
		}
	}

	public void Clear()
	{
		_goodAmounts.Clear();
		TotalAmount = 0;
	}

	private GoodAmount ClampGoodAmount(GoodAmount goodAmount)
	{
		int num = int.MaxValue - TotalAmount;
		if (num < goodAmount.Amount)
		{
			return new GoodAmount(goodAmount.GoodId, num);
		}
		return goodAmount;
	}

	private void AddInternal(GoodAmount goodAmount)
	{
		TotalAmount += goodAmount.Amount;
		if (!TryAddToExistingGood(goodAmount))
		{
			_goodAmounts.Add(new GoodAmount(goodAmount.GoodId, goodAmount.Amount));
		}
	}

	private bool TryAddToExistingGood(GoodAmount goodAmount)
	{
		for (int i = 0; i < _goodAmounts.Count; i++)
		{
			GoodAmount goodAmount2 = _goodAmounts[i];
			if (goodAmount2.GoodId == goodAmount.GoodId)
			{
				_goodAmounts[i] = new GoodAmount(goodAmount.GoodId, goodAmount.Amount + goodAmount2.Amount);
				return true;
			}
		}
		return false;
	}

	private static void AssertValueIsPositive(GoodAmount goodAmount)
	{
		Asserts.ValueIsInRange(goodAmount.Amount, 0, int.MaxValue, "GoodAmount");
	}
}
