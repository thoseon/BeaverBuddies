using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemBatchControl;

internal class ExportThresholdSliderFactory
{
	private readonly ILoc _loc;

	private readonly TooltipBlocker _tooltipBlocker;

	private readonly VisualElementLoader _visualElementLoader;

	public ExportThresholdSliderFactory(ILoc loc, TooltipBlocker tooltipBlocker, VisualElementLoader visualElementLoader)
	{
		_loc = loc;
		_tooltipBlocker = tooltipBlocker;
		_visualElementLoader = visualElementLoader;
	}

	public ExportThresholdSlider Create(Slider slider, GoodDistributionSetting goodDistributionSetting)
	{
		VisualElement tooltip = CreateExportThresholdSliderTooltip(slider);
		ExportThresholdSlider exportThresholdSlider = new ExportThresholdSlider(_loc, _tooltipBlocker, goodDistributionSetting, slider, tooltip);
		exportThresholdSlider.Initialize();
		return exportThresholdSlider;
	}

	private VisualElement CreateExportThresholdSliderTooltip(Slider slider)
	{
		string elementName = "Game/BatchControl/ExportThresholdSliderTooltip";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		slider.Q<VisualElement>("unity-dragger").Add(visualElement);
		visualElement.ToggleDisplayStyle(visible: false);
		return visualElement;
	}
}
