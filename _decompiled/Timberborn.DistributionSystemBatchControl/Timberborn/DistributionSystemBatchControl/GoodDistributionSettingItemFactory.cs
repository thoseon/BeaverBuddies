using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using Timberborn.DistributionSystemUI;
using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemBatchControl;

internal class GoodDistributionSettingItemFactory
{
	private readonly ExportThresholdSliderFactory _exportThresholdSliderFactory;

	private readonly ImportGoodIconFactory _importGoodIconFactory;

	private readonly ImportToggleFactory _importToggleFactory;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly Phrase _fillRatePhrase = Phrase.New().FormatPercentRounded();

	public GoodDistributionSettingItemFactory(ExportThresholdSliderFactory exportThresholdSliderFactory, ImportGoodIconFactory importGoodIconFactory, ImportToggleFactory importToggleFactory, ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader, ILoc loc)
	{
		_exportThresholdSliderFactory = exportThresholdSliderFactory;
		_importGoodIconFactory = importGoodIconFactory;
		_importToggleFactory = importToggleFactory;
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
		_loc = loc;
	}

	public GoodDistributionSettingItem Create(DistrictDistributableGoodProvider districtDistributableGoodProvider, GoodDistributionSetting goodDistributionSetting)
	{
		string elementName = "Game/BatchControl/GoodDistributionSettingItem";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		VisualElement parent = visualElement.Q<VisualElement>("ImportGoodIconWrapper");
		ImportGoodIcon importGoodIcon = _importGoodIconFactory.CreateImportGoodIcon(parent, goodDistributionSetting.GoodId);
		importGoodIcon.SetDistrictDistributableGoodProvider(districtDistributableGoodProvider);
		Slider slider = visualElement.Q<Slider>("ExportThresholdSlider");
		ExportThresholdSlider exportThresholdSlider = _exportThresholdSliderFactory.Create(slider, goodDistributionSetting);
		VisualElement parent2 = visualElement.Q<VisualElement>("ImportToggleWrapper");
		SliderToggle importToggle = _importToggleFactory.Create(parent2, goodDistributionSetting);
		Timberborn.CoreUI.ProgressBar progressBar = visualElement.Q<Timberborn.CoreUI.ProgressBar>("FillRateProgressBar");
		_tooltipRegistrar.RegisterUpdatable(progressBar, () => GetFillRateTooltip(districtDistributableGoodProvider, goodDistributionSetting));
		return new GoodDistributionSettingItem(visualElement, districtDistributableGoodProvider, goodDistributionSetting, importGoodIcon, exportThresholdSlider, importToggle, progressBar);
	}

	private string GetFillRateTooltip(DistrictDistributableGoodProvider districtDistributableGoodProvider, GoodDistributionSetting setting)
	{
		DistributableGood distributableGoodForExport = districtDistributableGoodProvider.GetDistributableGoodForExport(setting.GoodId);
		string arg = _loc.T(_fillRatePhrase, distributableGoodForExport.FillRate);
		return $"{distributableGoodForExport.Stock}/{distributableGoodForExport.Capacity} ({arg})";
	}
}
