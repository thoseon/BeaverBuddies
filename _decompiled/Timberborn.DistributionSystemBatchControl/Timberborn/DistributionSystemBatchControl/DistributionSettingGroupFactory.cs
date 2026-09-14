using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using Timberborn.Goods;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemBatchControl;

internal class DistributionSettingGroupFactory
{
	private readonly GoodDistributionSettingItemFactory _goodDistributionSettingItemFactory;

	private readonly VisualElementLoader _visualElementLoader;

	public DistributionSettingGroupFactory(GoodDistributionSettingItemFactory goodDistributionSettingItemFactory, VisualElementLoader visualElementLoader)
	{
		_goodDistributionSettingItemFactory = goodDistributionSettingItemFactory;
		_visualElementLoader = visualElementLoader;
	}

	public DistributionSettingGroup Create(GoodGroupSpec groupSpec, DistrictDistributionSetting districtDistributionSetting)
	{
		string elementName = "Game/BatchControl/DistributionSettingsGroup";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		visualElement.Q<Image>("Icon").sprite = groupSpec.Icon.Asset;
		List<GoodDistributionSettingItem> goodDistributionSettingItems = CreateItems(districtDistributionSetting, groupSpec.Id, visualElement.Q<VisualElement>("Items"));
		return new DistributionSettingGroup(visualElement, goodDistributionSettingItems);
	}

	private List<GoodDistributionSettingItem> CreateItems(DistrictDistributionSetting districtDistributionSetting, string groupId, VisualElement parent)
	{
		List<GoodDistributionSettingItem> list = new List<GoodDistributionSettingItem>();
		DistrictDistributableGoodProvider component = districtDistributionSetting.GetComponent<DistrictDistributableGoodProvider>();
		foreach (GoodDistributionSetting item in districtDistributionSetting.GetGoodDistributionSettingsForGroup(groupId))
		{
			GoodDistributionSettingItem goodDistributionSettingItem = _goodDistributionSettingItemFactory.Create(component, item);
			list.Add(goodDistributionSettingItem);
			parent.Add(goodDistributionSettingItem.Root);
		}
		return list;
	}
}
