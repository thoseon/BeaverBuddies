using System.Collections.Generic;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.SingletonSystem;

namespace Timberborn.DistributionSystemBatchControl;

internal class DistributionBatchControlTab : BatchControlTab
{
	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly DistributionBatchControlRowGroupFactory _distributionBatchControlRowGroupFactory;

	public override string TabNameLocKey => "BatchControl.Distribution";

	public override string TabImage => "Distribution";

	public override string BindingKey => "DistributionTab";

	protected override bool RemoveEmptyRowGroups => true;

	public DistributionBatchControlTab(VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, DistrictCenterRegistry districtCenterRegistry, DistributionBatchControlRowGroupFactory distributionBatchControlRowGroupFactory, EventBus eventBus)
		: base(visualElementLoader, batchControlDistrict, eventBus)
	{
		_districtCenterRegistry = districtCenterRegistry;
		_distributionBatchControlRowGroupFactory = distributionBatchControlRowGroupFactory;
	}

	protected override IEnumerable<BatchControlRowGroup> GetRowGroups(IEnumerable<EntityComponent> entities)
	{
		foreach (DistrictCenter finishedDistrictCenter in _districtCenterRegistry.FinishedDistrictCenters)
		{
			yield return _distributionBatchControlRowGroupFactory.Create(finishedDistrictCenter);
		}
	}
}
