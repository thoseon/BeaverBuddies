using Timberborn.BatchControl;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using Timberborn.EntitySystem;
using Timberborn.Goods;

namespace Timberborn.DistributionSystemBatchControl;

internal class DistributionSettingsRowItemFactory
{
	private readonly DistributionSettingGroupFactory _distributionSettingGroupFactory;

	private readonly GoodsGroupSpecService _goodsGroupSpecService;

	private readonly VisualElementLoader _visualElementLoader;

	private ReadOnlyList<GoodGroupSpec> GoodGroupSpecs => _goodsGroupSpecService.GoodGroupSpecs;

	public DistributionSettingsRowItemFactory(DistributionSettingGroupFactory distributionSettingGroupFactory, GoodsGroupSpecService goodsGroupSpecService, VisualElementLoader visualElementLoader)
	{
		_distributionSettingGroupFactory = distributionSettingGroupFactory;
		_goodsGroupSpecService = goodsGroupSpecService;
		_visualElementLoader = visualElementLoader;
	}

	public BatchControlRow Create(DistrictDistributionSetting districtDistributionSetting)
	{
		string elementName = "Game/BatchControl/DistributionSettingsRowItem";
		return new BatchControlRow(_visualElementLoader.LoadVisualElement(elementName), districtDistributionSetting.GetComponent<EntityComponent>(), CreateSettingGroups(districtDistributionSetting));
	}

	private IBatchControlRowItem[] CreateSettingGroups(DistrictDistributionSetting districtDistributionSetting)
	{
		IBatchControlRowItem[] array = new IBatchControlRowItem[GoodGroupSpecs.Count];
		for (int i = 0; i < GoodGroupSpecs.Count; i++)
		{
			GoodGroupSpec groupSpec = GoodGroupSpecs[i];
			array[i] = _distributionSettingGroupFactory.Create(groupSpec, districtDistributionSetting);
		}
		return array;
	}
}
