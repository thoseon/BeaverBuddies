using System.Collections.Generic;
using Timberborn.Goods;

namespace Timberborn.WaterWorkshops;

internal static class WaterGoodToWaterAmountConverter
{
	private static readonly string WaterId = "Water";

	private static readonly float WaterAmountConversion = 0.2f;

	public static float GetWaterAmount(IReadOnlyList<GoodAmountSpec> goods)
	{
		for (int i = 0; i < goods.Count; i++)
		{
			GoodAmountSpec goodAmountSpec = goods[i];
			if (goodAmountSpec.Id == WaterId)
			{
				return (float)goodAmountSpec.Amount * WaterAmountConversion;
			}
		}
		return 0f;
	}
}
