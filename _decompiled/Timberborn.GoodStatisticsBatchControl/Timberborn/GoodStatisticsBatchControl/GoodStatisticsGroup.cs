using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BatchControl;
using Timberborn.GoodStatisticsUI;
using Timberborn.GoodsSampling;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.GoodStatisticsBatchControl;

internal class GoodStatisticsGroup : IBatchControlRowItem, IInitializableBatchControlItem, IUpdatableBatchControlRowItem, IClearableBatchControlRowItem
{
	private readonly EventBus _eventBus;

	private readonly GoodStatisticsTypeSelector _goodStatisticsTypeSelector;

	private readonly BatchControlBoxTimeRangeController _batchControlBoxTimeRangeController;

	private readonly ImmutableArray<GoodStatisticsBatchControlItem> _goodStatisticsBatchControlItems;

	private bool _isDirty;

	public VisualElement Root { get; }

	public GoodStatisticsGroup(EventBus eventBus, GoodStatisticsTypeSelector goodStatisticsTypeSelector, BatchControlBoxTimeRangeController batchControlBoxTimeRangeController, VisualElement root, IEnumerable<GoodStatisticsBatchControlItem> goodStatisticsBatchControlItems)
	{
		_eventBus = eventBus;
		_goodStatisticsTypeSelector = goodStatisticsTypeSelector;
		_batchControlBoxTimeRangeController = batchControlBoxTimeRangeController;
		Root = root;
		_goodStatisticsBatchControlItems = goodStatisticsBatchControlItems.ToImmutableArray();
	}

	public void InitializeRowItem()
	{
		UpdateItems();
		_eventBus.Register(this);
	}

	public void UpdateRowItem()
	{
		if (_isDirty)
		{
			UpdateItems();
			_isDirty = false;
		}
	}

	public void ClearRowItem()
	{
		_eventBus.Unregister(this);
	}

	[OnEvent]
	public void OnGoodsSampled(GoodsSampledEvent goodsSampledEvent)
	{
		_isDirty = true;
	}

	[OnEvent]
	public void OnGoodStatisticsTypeChanged(GoodStatisticsTypeChangedEvent goodStatisticsTypeChangedEventEvent)
	{
		_isDirty = true;
	}

	[OnEvent]
	public void OnBatchControlBoxTimeRangeChanged(BatchControlBoxTimeRangeChangedEvent batchControlBoxTimeRangeChangedEvent)
	{
		_isDirty = true;
	}

	private void UpdateItems()
	{
		int daysTimeRange = _batchControlBoxTimeRangeController.GetDaysTimeRange();
		GoodStatisticType selectedType = _goodStatisticsTypeSelector.SelectedType;
		foreach (GoodStatisticsBatchControlItem goodStatisticsBatchControlItem in _goodStatisticsBatchControlItems)
		{
			goodStatisticsBatchControlItem.Update(daysTimeRange, selectedType);
		}
	}
}
