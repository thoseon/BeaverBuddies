using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using Timberborn.DistributionSystemUI;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemBatchControl;

internal class GoodDistributionSettingItem
{
	private readonly DistrictDistributableGoodProvider _districtDistributableGoodProvider;

	private readonly GoodDistributionSetting _setting;

	private readonly ImportGoodIcon _importGoodIcon;

	private readonly ExportThresholdSlider _exportThresholdSlider;

	private readonly SliderToggle _importToggle;

	private readonly Timberborn.CoreUI.ProgressBar _fillRateProgressBar;

	public VisualElement Root { get; }

	public GoodDistributionSettingItem(VisualElement root, DistrictDistributableGoodProvider districtDistributableGoodProvider, GoodDistributionSetting setting, ImportGoodIcon importGoodIcon, ExportThresholdSlider exportThresholdSlider, SliderToggle importToggle, Timberborn.CoreUI.ProgressBar fillRateProgressBar)
	{
		Root = root;
		_districtDistributableGoodProvider = districtDistributableGoodProvider;
		_setting = setting;
		_importGoodIcon = importGoodIcon;
		_exportThresholdSlider = exportThresholdSlider;
		_importToggle = importToggle;
		_fillRateProgressBar = fillRateProgressBar;
	}

	public void Update()
	{
		_importGoodIcon.Update();
		_exportThresholdSlider.Update();
		_importToggle.Update();
		DistributableGood distributableGoodForExport = _districtDistributableGoodProvider.GetDistributableGoodForExport(_setting.GoodId);
		_fillRateProgressBar.SetProgress(distributableGoodForExport.FillRate);
	}

	public void Clear()
	{
		_importGoodIcon.Clear();
		_exportThresholdSlider.Clear();
	}
}
