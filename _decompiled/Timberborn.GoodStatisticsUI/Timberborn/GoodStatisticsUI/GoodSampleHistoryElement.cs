using System;
using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.GoodsSampling;
using UnityEngine;

namespace Timberborn.GoodStatisticsUI;

public class GoodSampleHistoryElement
{
	private static readonly Color ChartYellow = new Color(0.62f, 0.53f, 0.35f);

	private static readonly Color ChartYellowHover = new Color(0.73f, 0.65f, 0.47f);

	private static readonly Color ChartGreen = new Color(0.32f, 0.49f, 0.45f);

	private static readonly Color ChartGreenHover = new Color(0.44f, 0.61f, 0.57f);

	private static readonly Color ChartRed = new Color(0.63f, 0.33f, 0.31f);

	private static readonly Color ChartRedHover = new Color(0.84f, 0.44f, 0.44f);

	private readonly GoodSampleHistory _goodSampleHistory;

	private readonly BarChart _barChart;

	private readonly List<IBarChartDataPoint> _values = new List<IBarChartDataPoint>();

	public GoodSampleHistoryElement(GoodSampleHistory goodSampleHistory, BarChart barChart)
	{
		_goodSampleHistory = goodSampleHistory;
		_barChart = barChart;
	}

	public void Update(int timeRange, GoodStatisticType statisticToShow)
	{
		int amount = Mathf.Min(timeRange, _barChart.MaxBarsAmount);
		float num = 0f;
		foreach (GoodSample goodSample in _goodSampleHistory.GetGoodSamples(timeRange, amount))
		{
			_values.Add(new GoodSampleDataPoint(_goodSampleHistory.GoodId, goodSample, statisticToShow));
			float maxValue = GetMaxValue(goodSample, statisticToShow);
			if (maxValue > num)
			{
				num = maxValue;
			}
		}
		_barChart.Update(_values, num, GetColor(statisticToShow), GetHoverColor(statisticToShow));
		_values.Clear();
	}

	private static float GetMaxValue(GoodSample goodSample, GoodStatisticType statisticToShow)
	{
		int num;
		switch (statisticToShow)
		{
		case GoodStatisticType.Stock:
			num = goodSample.Capacity;
			break;
		case GoodStatisticType.Production:
		case GoodStatisticType.Consumption:
			num = Mathf.Max(goodSample.Production, goodSample.Consumption);
			break;
		default:
			throw new ArgumentOutOfRangeException("statisticToShow", statisticToShow, null);
		}
		return num;
	}

	private static Color GetColor(GoodStatisticType statisticToShow)
	{
		return statisticToShow switch
		{
			GoodStatisticType.Stock => ChartYellow, 
			GoodStatisticType.Production => ChartGreen, 
			GoodStatisticType.Consumption => ChartRed, 
			_ => throw new ArgumentOutOfRangeException("statisticToShow", statisticToShow, null), 
		};
	}

	private static Color GetHoverColor(GoodStatisticType statisticToShow)
	{
		return statisticToShow switch
		{
			GoodStatisticType.Stock => ChartYellowHover, 
			GoodStatisticType.Production => ChartGreenHover, 
			GoodStatisticType.Consumption => ChartRedHover, 
			_ => throw new ArgumentOutOfRangeException("statisticToShow", statisticToShow, null), 
		};
	}
}
