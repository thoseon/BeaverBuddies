using System;
using Timberborn.CoreUI;
using Timberborn.GoodsSampling;

namespace Timberborn.GoodStatisticsUI;

public class GoodSampleDataPoint : IBarChartDataPoint
{
	public string GoodId { get; }

	public GoodSample Sample { get; }

	public GoodStatisticType StatisticType { get; }

	public float Value => StatisticType switch
	{
		GoodStatisticType.Stock => Sample.Stock, 
		GoodStatisticType.Production => Sample.Production, 
		GoodStatisticType.Consumption => Sample.Consumption, 
		_ => throw new ArgumentOutOfRangeException("StatisticType", StatisticType, null), 
	};

	public GoodSampleDataPoint(string goodId, GoodSample sample, GoodStatisticType statisticType)
	{
		GoodId = goodId;
		Sample = sample;
		StatisticType = statisticType;
	}
}
