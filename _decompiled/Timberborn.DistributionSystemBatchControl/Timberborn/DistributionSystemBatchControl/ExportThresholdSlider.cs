using System;
using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemBatchControl;

internal class ExportThresholdSlider
{
	private static readonly int DragMouseButton = 0;

	private static readonly float ExportThresholdSliderScale = 0.05f;

	private static readonly string TooltipLocKey = "Distribution.ExportThreshold";

	private static readonly string HighlightClass = "export-threshold-slider--highlighted";

	private readonly ILoc _loc;

	private readonly TooltipBlocker _tooltipBlocker;

	private readonly GoodDistributionSetting _setting;

	private readonly Slider _slider;

	private readonly VisualElement _tooltip;

	private Label _tooltipLabel;

	private bool _isDragged;

	private bool _isHovered;

	private bool _isTooltipShown;

	private readonly Phrase _exportThresholdPhrase = Phrase.New().FormatPercentRounded();

	private bool ShouldShowTooltip
	{
		get
		{
			if (!_isDragged)
			{
				return _isHovered;
			}
			return true;
		}
	}

	public ExportThresholdSlider(ILoc loc, TooltipBlocker tooltipBlocker, GoodDistributionSetting setting, Slider slider, VisualElement tooltip)
	{
		_loc = loc;
		_tooltipBlocker = tooltipBlocker;
		_setting = setting;
		_slider = slider;
		_tooltip = tooltip;
	}

	public void Initialize()
	{
		_tooltipLabel = _tooltip.Q<Label>("TooltipLabel");
		VisualElement visualElement = _slider.Q(null, BaseSlider<float>.draggerUssClassName);
		visualElement.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
		visualElement.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
		visualElement.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
		VisualElement visualElement2 = _slider.Q(null, BaseSlider<float>.dragContainerUssClassName);
		visualElement2.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
		visualElement2.RegisterCallback<MouseUpEvent>(OnMouseUp);
		_slider.lowValue = 0f;
		_slider.highValue = 1f;
		_slider.value = _setting.ExportThreshold;
		_slider.RegisterValueChangedCallback(OnSliderChanged);
	}

	public void Update()
	{
		UpdateSliderPosition();
		UpdateTooltipState();
	}

	public void Clear()
	{
		if (_isTooltipShown)
		{
			_tooltipBlocker.RemoveBlocker(this);
		}
	}

	private void OnMouseEnter(MouseEnterEvent evt)
	{
		if (evt.pressedButtons == 0)
		{
			_isHovered = true;
		}
	}

	private void OnMouseLeave(MouseLeaveEvent evt)
	{
		_isHovered = false;
	}

	private void OnMouseDown(MouseDownEvent evt)
	{
		if (evt.button == DragMouseButton)
		{
			_isDragged = true;
		}
	}

	private void OnMouseUp(MouseUpEvent evt)
	{
		if (evt.button == DragMouseButton)
		{
			_isDragged = false;
		}
	}

	private void OnSliderChanged(ChangeEvent<float> evt)
	{
		float exportThreshold = (float)Math.Round(evt.newValue / ExportThresholdSliderScale) * ExportThresholdSliderScale;
		_setting.SetExportThreshold(exportThreshold);
		UpdateSliderPosition();
		UpdateTooltipLabel();
	}

	private void UpdateTooltipLabel()
	{
		string param = _loc.T(_exportThresholdPhrase, _setting.ExportThreshold);
		_tooltipLabel.text = _loc.T(TooltipLocKey, param);
	}

	private void UpdateSliderPosition()
	{
		if (Math.Abs(_slider.value - _setting.ExportThreshold) > 0.0001f)
		{
			_slider.SetValueWithoutNotify(_setting.ExportThreshold);
		}
	}

	private void UpdateTooltipState()
	{
		if (ShouldShowTooltip && !_isTooltipShown)
		{
			ShowTooltip();
		}
		else if (!ShouldShowTooltip && _isTooltipShown)
		{
			HideTooltip();
		}
	}

	private void ShowTooltip()
	{
		UpdateTooltipLabel();
		_tooltip.ToggleDisplayStyle(visible: true);
		_isTooltipShown = true;
		_tooltipBlocker.AddBlocker(this);
		_slider.AddToClassList(HighlightClass);
	}

	private void HideTooltip()
	{
		_tooltip.ToggleDisplayStyle(visible: false);
		_isTooltipShown = false;
		_tooltipBlocker.RemoveBlocker(this);
		_slider.RemoveFromClassList(HighlightClass);
	}
}
