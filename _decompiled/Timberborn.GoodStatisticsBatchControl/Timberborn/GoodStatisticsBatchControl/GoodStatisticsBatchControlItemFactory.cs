using Timberborn.CoreUI;
using Timberborn.GoodStatisticsUI;
using Timberborn.Goods;
using Timberborn.GoodsSampling;
using Timberborn.GoodsUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.GoodStatisticsBatchControl;

internal class GoodStatisticsBatchControlItemFactory
{
	private static readonly string TimestampLocKey = "Weather.CycleAndDayLong";

	private static readonly string GoodDataLocKeyPrefix = "GoodsStatistics.";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly GoodDescriber _goodDescriber;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly IGoodService _goodService;

	private readonly ILoc _loc;

	public GoodStatisticsBatchControlItemFactory(VisualElementLoader visualElementLoader, GoodDescriber goodDescriber, ITooltipRegistrar tooltipRegistrar, IGoodService goodService, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_goodDescriber = goodDescriber;
		_tooltipRegistrar = tooltipRegistrar;
		_goodService = goodService;
		_loc = loc;
	}

	public GoodStatisticsBatchControlItem Create(GoodSampleHistory goodSampleHistory)
	{
		string elementName = "Game/BatchControl/GoodStatisticsBatchControlItem";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		visualElement.Q<Image>("GoodIcon").sprite = _goodDescriber.GetDescribedGood(goodSampleHistory.GoodId).Icon;
		BarChart barChart = visualElement.Q<BarChart>("GoodSampleHistoryChart");
		GoodSampleHistoryElement goodSampleHistoryElement = new GoodSampleHistoryElement(goodSampleHistory, barChart);
		RegisterTooltip(barChart);
		return new GoodStatisticsBatchControlItem(visualElement, goodSampleHistoryElement);
	}

	private void RegisterTooltip(BarChart barChart)
	{
		_tooltipRegistrar.RegisterInstantUpdatable(barChart, () => GetTooltip(barChart));
	}

	private VisualElement GetTooltip(BarChart barChart)
	{
		IBarChartDataPoint hoveredDataPoint = barChart.HoveredDataPoint;
		if (hoveredDataPoint != null)
		{
			GoodSampleDataPoint goodSampleDataPoint = (GoodSampleDataPoint)hoveredDataPoint;
			if (goodSampleDataPoint.Sample.Cycle > 0)
			{
				VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/GoodSampleTooltip");
				string goodId = goodSampleDataPoint.GoodId;
				visualElement.Q<Label>("Name").text = _goodService.GetGood(goodId).PluralDisplayName.Value;
				GoodSample sample = goodSampleDataPoint.Sample;
				visualElement.Q<Label>("Date").text = _loc.T(TimestampLocKey, sample.Cycle, sample.Day);
				string key = GoodDataLocKeyPrefix + goodSampleDataPoint.StatisticType;
				visualElement.Q<Label>("Data").text = _loc.T(key, goodSampleDataPoint.Value);
				return visualElement;
			}
		}
		return null;
	}
}
