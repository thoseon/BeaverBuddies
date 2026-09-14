using System;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using UnityEngine;

namespace Timberborn.Carrying;

public class CarryAmountCalculator
{
	private readonly IGoodService _goodService;

	public CarryAmountCalculator(IGoodService goodService)
	{
		_goodService = goodService;
	}

	public GoodAmount AmountToCarry(int liftingCapacity, string goodId, IAmountProvider input, IAmountProvider output)
	{
		GoodAmount good = new GoodAmount(goodId, output.UnreservedAmountInStock(goodId));
		return AmountToCarry(liftingCapacity, good, input);
	}

	public GoodAmount AmountToCarry(int liftingCapacity, GoodAmount good, IAmountProvider input)
	{
		GoodSpec good2 = _goodService.GetGood(good.GoodId);
		int num = Math.Max(liftingCapacity / good2.Weight, 1);
		int num2 = input.UnreservedCapacity(good2.Id);
		int amount = Mathf.Min(num, good.Amount, num2);
		return new GoodAmount(good.GoodId, amount);
	}
}
