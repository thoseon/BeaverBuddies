using System;
using Timberborn.Common;
using Timberborn.Goods;
using UnityEngine;

namespace Timberborn.InventorySystem;

public class InventoryFillCalculator
{
	public float GetInputFillPercentage(Inventory inventory)
	{
		return GetInventoryFillPercentage(inventory, inventory.InputGoods, onlyInStock: false).lowest;
	}

	public float GetOutputFillPercentage(Inventory inventory)
	{
		return GetInventoryFillPercentage(inventory, inventory.OutputGoods, onlyInStock: false).average;
	}

	public float GetInStockOutputFillPercentage(Inventory inventory)
	{
		return GetInventoryFillPercentage(inventory, inventory.OutputGoods, onlyInStock: true).average;
	}

	private static (float average, float lowest) GetInventoryFillPercentage(Inventory inventory, ReadOnlyHashSet<string> goods, bool onlyInStock)
	{
		float num = 1f;
		int num2 = 0;
		int num3 = 0;
		foreach (StorableGoodAmount allowedGood in inventory.AllowedGoods)
		{
			string goodId = allowedGood.StorableGood.GoodId;
			if (!goods.Contains(goodId))
			{
				continue;
			}
			int num4 = inventory.LimitedAmount(goodId);
			if (num4 <= 0)
			{
				continue;
			}
			int num5 = inventory.AmountInStock(goodId);
			if (!onlyInStock || num5 > 0)
			{
				num2 += num4;
				num3 += num5;
				float num6 = (float)num5 / (float)num4;
				if (num6 < num)
				{
					num = num6;
				}
			}
		}
		num2 = Math.Min(num2, inventory.Capacity);
		return (average: (num2 == 0) ? 0f : Mathf.Clamp01((float)num3 / (float)num2), lowest: num);
	}
}
