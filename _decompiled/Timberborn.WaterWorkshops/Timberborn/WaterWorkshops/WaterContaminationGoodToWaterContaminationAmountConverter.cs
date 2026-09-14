using System.Collections.Generic;
using Timberborn.Goods;

namespace Timberborn.WaterWorkshops;

internal static class WaterContaminationGoodToWaterContaminationAmountConverter
{
	private static readonly string WaterContaminationId = "Badwater";

	private static readonly float WaterContaminationAmountConversion = 0.2f;

	public static float GetWaterContaminationAmount(IReadOnlyList<GoodAmountSpec> goods)
	{
		for (int i = 0; i < goods.Count; i++)
		{
			GoodAmountSpec goodAmountSpec = goods[i];
			if (goodAmountSpec.Id == WaterContaminationId)
			{
				return (float)goodAmountSpec.Amount * WaterContaminationAmountConversion;
			}
		}
		return 0f;
	}
}
