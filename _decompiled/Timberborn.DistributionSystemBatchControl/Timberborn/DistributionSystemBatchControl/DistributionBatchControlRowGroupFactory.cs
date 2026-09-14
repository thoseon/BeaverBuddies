using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.GameDistrictsBatchControl;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemBatchControl;

internal class DistributionBatchControlRowGroupFactory
{
	private readonly BatchControlRowGroupFactory _batchControlRowGroupFactory;

	private readonly DistrictCenterRowItemFactory _districtCenterRowItemFactory;

	private readonly DistributionSettingsRowItemFactory _distributionSettingsRowItemFactory;

	private readonly DistrictDistributionControlRowItemFactory _districtDistributionControlRowItemFactory;

	private readonly VisualElementLoader _visualElementLoader;

	public DistributionBatchControlRowGroupFactory(BatchControlRowGroupFactory batchControlRowGroupFactory, DistrictCenterRowItemFactory districtCenterRowItemFactory, DistributionSettingsRowItemFactory distributionSettingsRowItemFactory, DistrictDistributionControlRowItemFactory districtDistributionControlRowItemFactory, VisualElementLoader visualElementLoader)
	{
		_batchControlRowGroupFactory = batchControlRowGroupFactory;
		_districtCenterRowItemFactory = districtCenterRowItemFactory;
		_distributionSettingsRowItemFactory = distributionSettingsRowItemFactory;
		_districtDistributionControlRowItemFactory = districtDistributionControlRowItemFactory;
		_visualElementLoader = visualElementLoader;
	}

	public BatchControlRowGroup Create(DistrictCenter districtCenter)
	{
		string elementName = "Game/BatchControl/BatchControlRow";
		VisualElement root = _visualElementLoader.LoadVisualElement(elementName);
		DistrictDistributionSetting component = districtCenter.GetComponent<DistrictDistributionSetting>();
		IBatchControlRowItem batchControlRowItem = _districtCenterRowItemFactory.Create(districtCenter);
		IBatchControlRowItem batchControlRowItem2 = _districtDistributionControlRowItemFactory.Create(component);
		BatchControlRow header = new BatchControlRow(root, districtCenter.GetComponent<EntityComponent>(), batchControlRowItem, batchControlRowItem2);
		BatchControlRowGroup batchControlRowGroup = _batchControlRowGroupFactory.CreateUnsorted(header);
		batchControlRowGroup.AddRow(_distributionSettingsRowItemFactory.Create(component));
		return batchControlRowGroup;
	}
}
