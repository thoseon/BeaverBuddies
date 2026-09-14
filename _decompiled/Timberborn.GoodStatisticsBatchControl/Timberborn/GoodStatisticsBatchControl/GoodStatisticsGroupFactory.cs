using System.Collections.Generic;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.Goods;
using Timberborn.GoodsSampling;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.GoodStatisticsBatchControl;

internal class GoodStatisticsGroupFactory
{
	private readonly GoodStatisticsBatchControlItemFactory _goodStatisticsBatchControlItemFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly IGoodService _goodService;

	private readonly EventBus _eventBus;

	private readonly GoodStatisticsTypeSelector _goodStatisticsTypeSelector;

	private readonly BatchControlBoxTimeRangeController _batchControlBoxTimeRangeController;

	public GoodStatisticsGroupFactory(GoodStatisticsBatchControlItemFactory goodStatisticsBatchControlItemFactory, VisualElementLoader visualElementLoader, IGoodService goodService, EventBus eventBus, GoodStatisticsTypeSelector goodStatisticsTypeSelector, BatchControlBoxTimeRangeController batchControlBoxTimeRangeController)
	{
		_goodStatisticsBatchControlItemFactory = goodStatisticsBatchControlItemFactory;
		_visualElementLoader = visualElementLoader;
		_goodService = goodService;
		_eventBus = eventBus;
		_goodStatisticsTypeSelector = goodStatisticsTypeSelector;
		_batchControlBoxTimeRangeController = batchControlBoxTimeRangeController;
	}

	public GoodStatisticsGroup Create(GoodGroupSpec goodGroupSpec, GoodSamplingRegistry goodSamplingRegistry)
	{
		string elementName = "Game/BatchControl/GoodStatisticsGroup";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		visualElement.Q<Image>("Icon").sprite = goodGroupSpec.Icon.Asset;
		IEnumerable<GoodStatisticsBatchControlItem> goodStatisticsBatchControlItems = CreateItems(goodGroupSpec.Id, visualElement.Q<VisualElement>("Items"), goodSamplingRegistry);
		return new GoodStatisticsGroup(_eventBus, _goodStatisticsTypeSelector, _batchControlBoxTimeRangeController, visualElement, goodStatisticsBatchControlItems);
	}

	private IEnumerable<GoodStatisticsBatchControlItem> CreateItems(string groupId, VisualElement parent, GoodSamplingRegistry goodSamplingRegistry)
	{
		foreach (string item in _goodService.GetGoodsForGroup(groupId))
		{
			GoodSampleHistory goodSampleHistory = goodSamplingRegistry.GetGoodSampleHistory(item);
			GoodStatisticsBatchControlItem goodStatisticsBatchControlItem = _goodStatisticsBatchControlItemFactory.Create(goodSampleHistory);
			parent.Add(goodStatisticsBatchControlItem.Root);
			yield return goodStatisticsBatchControlItem;
		}
	}
}
