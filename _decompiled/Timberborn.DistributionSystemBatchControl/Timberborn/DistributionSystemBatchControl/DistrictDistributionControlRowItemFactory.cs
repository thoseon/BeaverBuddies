using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemBatchControl;

internal class DistrictDistributionControlRowItemFactory
{
	private static readonly string ResetLocKey = "Distribution.Reset";

	private static readonly string ExportAllLocKey = "Distribution.ExportAll";

	private static readonly string ExportNoneLocKey = "Distribution.ExportNone";

	private static readonly string ImportAutoAllLocKey = "Distribution.ImportAutoAll";

	private static readonly string ImportDisabledAllLocKey = "Distribution.ImportDisabledAll";

	private static readonly string ImportForcedAllLocKey = "Distribution.ImportForcedAll";

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	public DistrictDistributionControlRowItemFactory(ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader)
	{
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
	}

	public IBatchControlRowItem Create(DistrictDistributionSetting districtDistributionSetting)
	{
		string elementName = "Game/BatchControl/DistrictDistributionControlRowItem";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		Button button = visualElement.Q<Button>("Reset");
		button.RegisterCallback<ClickEvent>(delegate
		{
			districtDistributionSetting.ResetToDefault();
		});
		_tooltipRegistrar.RegisterLocalizable(button, ResetLocKey);
		Button button2 = visualElement.Q<Button>("ExportAll");
		button2.RegisterCallback<ClickEvent>(delegate
		{
			districtDistributionSetting.SetDistrictExportThreshold(0);
		});
		_tooltipRegistrar.RegisterLocalizable(button2, ExportAllLocKey);
		Button button3 = visualElement.Q<Button>("ExportNone");
		button3.RegisterCallback<ClickEvent>(delegate
		{
			districtDistributionSetting.SetDistrictExportThreshold(1);
		});
		_tooltipRegistrar.RegisterLocalizable(button3, ExportNoneLocKey);
		Button button4 = visualElement.Q<Button>("ImportDisabledAll");
		button4.RegisterCallback<ClickEvent>(delegate
		{
			districtDistributionSetting.SetDistrictImportOption(ImportOption.Disabled);
		});
		_tooltipRegistrar.RegisterLocalizable(button4, ImportDisabledAllLocKey);
		Button button5 = visualElement.Q<Button>("ImportAutoAll");
		button5.RegisterCallback<ClickEvent>(delegate
		{
			districtDistributionSetting.SetDistrictImportOption(ImportOption.Auto);
		});
		_tooltipRegistrar.RegisterLocalizable(button5, ImportAutoAllLocKey);
		Button button6 = visualElement.Q<Button>("ImportForcedAll");
		button6.RegisterCallback<ClickEvent>(delegate
		{
			districtDistributionSetting.SetDistrictImportOption(ImportOption.Forced);
		});
		_tooltipRegistrar.RegisterLocalizable(button6, ImportForcedAllLocKey);
		return new EmptyBatchControlRowItem(visualElement);
	}
}
