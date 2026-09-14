using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class UISettingsController
{
	private static readonly bool ShouldShowSteamKeyboardDropdown = true;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly OnScreenKeyboardDropdownProvider _onScreenKeyboardDropdownProvider;

	private readonly UISettings _uiSettings;

	private readonly UIScaler _uiScaler;

	private Toggle _showFPSToggle;

	private Toggle _runInBackgroundToggle;

	private SliderInt _uiScaleFactorSlider;

	private Label _uiScaleFactorValueLabel;

	private Dropdown _onScreenKeyboardDropdown;

	public UISettingsController(DropdownItemsSetter dropdownItemsSetter, OnScreenKeyboardDropdownProvider onScreenKeyboardDropdownProvider, UISettings uiSettings, UIScaler uiScaler)
	{
		_dropdownItemsSetter = dropdownItemsSetter;
		_onScreenKeyboardDropdownProvider = onScreenKeyboardDropdownProvider;
		_uiSettings = uiSettings;
		_uiScaler = uiScaler;
	}

	public void Initialize(VisualElement root)
	{
		_showFPSToggle = root.Q<Toggle>("ShowFPS");
		_showFPSToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_uiSettings.ShowFps = v.newValue;
		});
		_runInBackgroundToggle = root.Q<Toggle>("RunInBackground");
		_runInBackgroundToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_uiSettings.RunInBackground = v.newValue;
		});
		VisualElement e = root.Q<VisualElement>("UIScaleFactor");
		_uiScaleFactorSlider = e.Q<SliderInt>("Slider");
		_uiScaleFactorValueLabel = e.Q<Label>("Value");
		_uiScaleFactorSlider.lowValue = RoundToInt(UIScaler.MinScaleFactor);
		_uiScaleFactorSlider.highValue = RoundToInt(UIScaler.MaxScaleFactor);
		_uiScaleFactorSlider.RegisterValueChangedCallback(delegate(ChangeEvent<int> v)
		{
			_uiSettings.UIScaleFactor = _uiScaler.ClampScaleFactor((float)v.newValue * UISettings.UIScaleStep);
			_uiScaleFactorValueLabel.text = ((float)v.newValue * UISettings.UIScaleStep).ToString("P0");
		});
		_onScreenKeyboardDropdown = root.Q<Dropdown>("OnScreenKeyboard");
		_onScreenKeyboardDropdown.ToggleDisplayStyle(ShouldShowSteamKeyboardDropdown);
		if (ShouldShowSteamKeyboardDropdown)
		{
			_dropdownItemsSetter.SetItems(_onScreenKeyboardDropdown, _onScreenKeyboardDropdownProvider);
		}
	}

	public void Update()
	{
		_showFPSToggle.SetValueWithoutNotify(_uiSettings.ShowFps);
		_runInBackgroundToggle.SetValueWithoutNotify(_uiSettings.RunInBackground);
		float value = _uiScaler.ClampScaleFactor(_uiSettings.UIScaleFactor);
		_uiScaleFactorSlider.SetValueWithoutNotify(RoundToInt(value));
		_uiScaleFactorValueLabel.text = value.ToString("P0");
	}

	private static int RoundToInt(float value)
	{
		return Mathf.RoundToInt(value / UISettings.UIScaleStep);
	}
}
