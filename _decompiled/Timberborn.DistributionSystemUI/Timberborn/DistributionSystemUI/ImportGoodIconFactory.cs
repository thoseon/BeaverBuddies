using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemUI;

public class ImportGoodIconFactory
{
	private readonly GoodDescriber _goodDescriber;

	private readonly IGoodService _goodService;

	private readonly GoodsGroupSpecService _goodsGroupSpecService;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	public ImportGoodIconFactory(GoodDescriber goodDescriber, IGoodService goodService, GoodsGroupSpecService goodsGroupSpecService, ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader)
	{
		_goodDescriber = goodDescriber;
		_goodService = goodService;
		_goodsGroupSpecService = goodsGroupSpecService;
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
	}

	public IEnumerable<ImportGoodIcon> CreateImportGoods(VisualElement parent)
	{
		List<ImportGoodIcon> list = new List<ImportGoodIcon>();
		foreach (GoodGroupSpec goodGroupSpec in _goodsGroupSpecService.GoodGroupSpecs)
		{
			list.AddRange(CreateImportGoodsGroup(parent, goodGroupSpec));
		}
		return list;
	}

	public ImportGoodIcon CreateImportGoodIcon(VisualElement parent, string goodId)
	{
		string elementName = "Game/ImportGoodIcon";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		parent.Add(visualElement);
		Image image = visualElement.Q<Image>("Icon");
		DescribedGood describedGood = _goodDescriber.GetDescribedGood(goodId);
		image.sprite = describedGood.Icon;
		VisualElement importableIcon = visualElement.Q<VisualElement>("ImportableIcon");
		VisualElement nonImportableIcon = visualElement.Q<VisualElement>("NonImportableIcon");
		ImportGoodIcon importGoodIcon = new ImportGoodIcon(goodId, importableIcon, nonImportableIcon);
		_tooltipRegistrar.Register(image, () => GetTooltip(importGoodIcon, goodId, describedGood.DisplayName));
		return importGoodIcon;
	}

	private IEnumerable<ImportGoodIcon> CreateImportGoodsGroup(VisualElement parent, GoodGroupSpec groupSpec)
	{
		string elementName = "Game/EntityPanel/ImportGoodsGroup";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		visualElement.Q<Image>("Icon").sprite = groupSpec.Icon.Asset;
		parent.Add(visualElement);
		VisualElement iconsParent = visualElement.Q<VisualElement>("Items");
		foreach (string item in _goodService.GetGoodsForGroup(groupSpec.Id))
		{
			yield return CreateImportGoodIcon(iconsParent, item);
		}
	}

	private VisualElement GetTooltip(ImportGoodIcon importGoodIcon, string goodId, string goodDisplayName)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/ImportGoodIconTooltip");
		visualElement.Q<Label>("GoodLabel").text = goodDisplayName;
		DistrictDistributableGoodProvider districtDistributableGoodProvider = importGoodIcon.DistrictDistributableGoodProvider;
		bool flag = districtDistributableGoodProvider.IsImportEnabled(goodId);
		ImportOption goodImportOption = districtDistributableGoodProvider.GetGoodImportOption(goodId);
		visualElement.Q<VisualElement>("DisabledInfo").ToggleDisplayStyle(goodImportOption == ImportOption.Disabled);
		visualElement.Q<VisualElement>("ForcedInfo").ToggleDisplayStyle(goodImportOption == ImportOption.Forced);
		visualElement.Q<VisualElement>("ImportableInfo").ToggleDisplayStyle((goodImportOption == ImportOption.Auto) & flag);
		visualElement.Q<VisualElement>("NonImportableInfo").ToggleDisplayStyle(goodImportOption == ImportOption.Auto && !flag);
		return visualElement;
	}
}
