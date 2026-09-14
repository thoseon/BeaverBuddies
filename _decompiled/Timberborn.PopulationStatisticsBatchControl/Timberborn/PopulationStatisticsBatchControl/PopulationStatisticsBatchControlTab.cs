using System.Collections.Generic;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.SingletonSystem;

namespace Timberborn.PopulationStatisticsBatchControl;

internal class PopulationStatisticsBatchControlTab : BatchControlTab
{
	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly PopulationStatisticsBatchControlRowGroupFactory _populationStatisticsBatchControlRowGroupFactory;

	private readonly TimeRangeDropdownProviderFactory _timeRangeDropdownProviderFactory;

	private TimeRangeDropdownProvider _timeRangeDropdownProvider;

	public override TimeRangeDropdownProvider TimeRangeDropdownProvider => _timeRangeDropdownProvider;

	public override string TabNameLocKey => "BatchControl.PopulationStatistics";

	public override string TabImage => "PopulationStatistics";

	public override string BindingKey => "PopulationStatisticsTab";

	protected override bool RemoveEmptyRowGroups => true;

	public PopulationStatisticsBatchControlTab(VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, DistrictCenterRegistry districtCenterRegistry, PopulationStatisticsBatchControlRowGroupFactory populationStatisticsBatchControlRowGroupFactory, EventBus eventBus, TimeRangeDropdownProviderFactory timeRangeDropdownProviderFactory)
		: base(visualElementLoader, batchControlDistrict, eventBus)
	{
		_districtCenterRegistry = districtCenterRegistry;
		_populationStatisticsBatchControlRowGroupFactory = populationStatisticsBatchControlRowGroupFactory;
		_timeRangeDropdownProviderFactory = timeRangeDropdownProviderFactory;
	}

	protected override void Initialize()
	{
		_timeRangeDropdownProvider = _timeRangeDropdownProviderFactory.Create(100);
	}

	protected override IEnumerable<BatchControlRowGroup> GetRowGroups(IEnumerable<EntityComponent> entities)
	{
		yield return _populationStatisticsBatchControlRowGroupFactory.CreateGlobal();
		foreach (DistrictCenter finishedDistrictCenter in _districtCenterRegistry.FinishedDistrictCenters)
		{
			yield return _populationStatisticsBatchControlRowGroupFactory.Create(finishedDistrictCenter);
		}
	}
}
