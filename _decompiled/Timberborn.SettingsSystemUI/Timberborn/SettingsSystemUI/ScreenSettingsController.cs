using Timberborn.DropdownSystem;
using Timberborn.ScreenSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class ScreenSettingsController
{
	private static readonly int ResolutionScaleSliderMultiplier = 20;

	private static readonly int MinBrightness = 50;

	private static readonly int MaxBrightness = 125;

	private readonly ScreenSettings _screenSettings;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly ScreenResolutionDropdownProvider _screenResolutionDropdownProvider;

	private readonly VSyncDropdownProvider _vSyncDropdownProvider;

	private readonly FrameRateLimitDropdownProvider _frameRateLimitDropdownProvider;

	private Toggle _fullScreenToggle;

	private Dropdown _screenResolutionDropdown;

	private SliderInt _resolutionScaleSlider;

	private Label _resolutionScaleValueLabel;

	private SliderInt _brightnessSlider;

	private Label _brightnessValueLabel;

	private Dropdown _vSyncDropdown;

	private Dropdown _frameRateLimitDropdown;

	public ScreenSettingsController(ScreenSettings screenSettings, DropdownItemsSetter dropdownItemsSetter, ScreenResolutionDropdownProvider screenResolutionDropdownProvider, VSyncDropdownProvider vSyncDropdownProvider, FrameRateLimitDropdownProvider frameRateLimitDropdownProvider)
	{
		_screenSettings = screenSettings;
		_dropdownItemsSetter = dropdownItemsSetter;
		_screenResolutionDropdownProvider = screenResolutionDropdownProvider;
		_vSyncDropdownProvider = vSyncDropdownProvider;
		_frameRateLimitDropdownProvider = frameRateLimitDropdownProvider;
	}

	public void Initialize(VisualElement root)
	{
		_fullScreenToggle = root.Q<Toggle>("FullScreen");
		_fullScreenToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_screenSettings.FullScreen = v.newValue;
		});
		_screenResolutionDropdown = root.Q<Dropdown>("ScreenResolution");
		VisualElement e = root.Q<VisualElement>("ResolutionScale");
		_resolutionScaleSlider = e.Q<SliderInt>("Slider");
		_resolutionScaleValueLabel = e.Q<Label>("Value");
		_resolutionScaleSlider.RegisterValueChangedCallback(OnResolutionScaleChanged);
		_resolutionScaleSlider.lowValue = 5;
		_resolutionScaleSlider.highValue = ResolutionScaleSliderMultiplier;
		VisualElement e2 = root.Q<VisualElement>("Brightness");
		_brightnessSlider = e2.Q<SliderInt>("Slider");
		_brightnessValueLabel = e2.Q<Label>("Value");
		_brightnessSlider.RegisterValueChangedCallback(OnBrightnessChanged);
		_brightnessSlider.lowValue = MinBrightness;
		_brightnessSlider.highValue = MaxBrightness;
		_vSyncDropdown = root.Q<Dropdown>("VSync");
		_vSyncDropdown.ValueChanged += delegate
		{
			UpdateFrameRateLimit();
		};
		_frameRateLimitDropdown = root.Q<Dropdown>("FrameRateLimit");
	}

	public void Update()
	{
		_fullScreenToggle.SetValueWithoutNotify(_screenSettings.FullScreen);
		_dropdownItemsSetter.SetItems(_screenResolutionDropdown, _screenResolutionDropdownProvider);
		_resolutionScaleSlider.SetValueWithoutNotify(Mathf.RoundToInt(_screenSettings.ResolutionScale * (float)ResolutionScaleSliderMultiplier));
		_brightnessSlider.SetValueWithoutNotify(Mathf.RoundToInt(_screenSettings.Brightness * 100f));
		_dropdownItemsSetter.SetItems(_vSyncDropdown, _vSyncDropdownProvider);
		_dropdownItemsSetter.SetItems(_frameRateLimitDropdown, _frameRateLimitDropdownProvider);
		UpdateSliderLabels();
		UpdateFrameRateLimit();
	}

	public void Clear()
	{
		_screenResolutionDropdown.ClearItems();
		_vSyncDropdown.ClearItems();
		_frameRateLimitDropdown.ClearItems();
	}

	private void OnResolutionScaleChanged(ChangeEvent<int> v)
	{
		float resolutionScale = (float)v.newValue / (float)ResolutionScaleSliderMultiplier;
		_screenSettings.ResolutionScale = resolutionScale;
		UpdateSliderLabels();
	}

	private void OnBrightnessChanged(ChangeEvent<int> v)
	{
		float brightness = (float)v.newValue / 100f;
		_screenSettings.Brightness = brightness;
		UpdateSliderLabels();
	}

	private void UpdateSliderLabels()
	{
		_resolutionScaleValueLabel.text = _screenSettings.ResolutionScale.ToString("P0");
		_brightnessValueLabel.text = _screenSettings.Brightness.ToString("P0");
	}

	private void UpdateFrameRateLimit()
	{
		_frameRateLimitDropdown.SetEnabled(_screenSettings.VSyncCount == 0);
		_frameRateLimitDropdown.UpdateSelectedValue();
	}
}
