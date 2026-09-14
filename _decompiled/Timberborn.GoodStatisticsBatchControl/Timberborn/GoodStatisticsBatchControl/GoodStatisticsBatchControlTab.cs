using System.Collections.Generic;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.GoodStatisticsBatchControl;

internal class GoodStatisticsBatchControlTab : BatchControlTab
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly GoodStatisticsBatchControlRowGroupFactory _goodStatisticsBatchControlRowGroupFactory;

	private readonly GoodStatisticsTypeSelector _goodStatisticsTypeSelector;

	private readonly TimeRangeDropdownProviderFactory _timeRangeDropdownProviderFactory;

	private TimeRangeDropdownProvider _timeRangeDropdownProvider;

	public override string TabNameLocKey => "BatchControl.GoodStatistics";

	public override string TabImage => "GoodStatistics";

	public override string BindingKey => "GoodStatisticsTab";

	public override TimeRangeDropdownProvider TimeRangeDropdownProvider => _timeRangeDropdownProvider;

	protected override bool RemoveEmptyRowGroups => true;

	public GoodStatisticsBatchControlTab(VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, DistrictCenterRegistry districtCenterRegistry, GoodStatisticsBatchControlRowGroupFactory goodStatisticsBatchControlRowGroupFactory, EventBus eventBus, GoodStatisticsTypeSelector goodStatisticsTypeSelector, TimeRangeDropdownProviderFactory timeRangeDropdownProviderFactory)
		: base(visualElementLoader, batchControlDistrict, eventBus)
	{
		_visualElementLoader = visualElementLoader;
		_districtCenterRegistry = districtCenterRegistry;
		_goodStatisticsBatchControlRowGroupFactory = goodStatisticsBatchControlRowGroupFactory;
		_goodStatisticsTypeSelector = goodStatisticsTypeSelector;
		_timeRangeDropdownProviderFactory = timeRangeDropdownProviderFactory;
	}

	protected override void Initialize()
	{
		_timeRangeDropdownProvider = _timeRangeDropdownProviderFactory.Create(10);
	}

	protected override IEnumerable<BatchControlRowGroup> GetRowGroups(IEnumerable<EntityComponent> entities)
	{
		yield return _goodStatisticsBatchControlRowGroupFactory.CreateGlobal();
		foreach (DistrictCenter finishedDistrictCenter in _districtCenterRegistry.FinishedDistrictCenters)
		{
			yield return _goodStatisticsBatchControlRowGroupFactory.Create(finishedDistrictCenter);
		}
	}

	protected override VisualElement GetHeader()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/BatchControl/GoodStatisticsHeader");
		_goodStatisticsTypeSelector.Initialize(visualElement);
		return visualElement;
	}
}
