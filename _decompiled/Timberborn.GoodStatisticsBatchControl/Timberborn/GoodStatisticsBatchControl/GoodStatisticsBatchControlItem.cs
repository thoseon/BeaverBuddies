using Timberborn.GoodStatisticsUI;
using UnityEngine.UIElements;

namespace Timberborn.GoodStatisticsBatchControl;

internal class GoodStatisticsBatchControlItem
{
	private readonly GoodSampleHistoryElement _goodSampleHistoryElement;

	public VisualElement Root { get; }

	public GoodStatisticsBatchControlItem(VisualElement root, GoodSampleHistoryElement goodSampleHistoryElement)
	{
		Root = root;
		_goodSampleHistoryElement = goodSampleHistoryElement;
	}

	public void Update(int timeRange, GoodStatisticType statisticToShow)
	{
		_goodSampleHistoryElement.Update(timeRange, statisticToShow);
	}
}
