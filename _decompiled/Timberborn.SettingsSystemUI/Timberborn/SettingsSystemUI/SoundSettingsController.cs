using System;
using Timberborn.SoundSettingsSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class SoundSettingsController
{
	private static readonly float MinVolume = 0f;

	private static readonly float MaxVolume = 1f;

	private readonly SoundSettings _soundSettings;

	private TextElement _masterVolumeValueLabel;

	private Slider _masterVolumeSlider;

	private Label _musicVolumeValueLabel;

	private Slider _musicVolumeSlider;

	private Label _environmentVolumeValueLabel;

	private Slider _environmentVolumeSlider;

	private Label _uiVolumeElementValueLabel;

	private Slider _uiVolumeElementSlider;

	private Toggle _muteWhenMinimizedToggle;

	public SoundSettingsController(SoundSettings soundSettings)
	{
		_soundSettings = soundSettings;
	}

	public void Initialize(VisualElement root)
	{
		VisualElement visualElement = root.Q<VisualElement>("MasterVolume");
		_masterVolumeValueLabel = visualElement.Q<Label>("Value");
		_masterVolumeSlider = InitializeSlider(visualElement, _masterVolumeValueLabel, delegate(float v)
		{
			_soundSettings.MasterVolume = v;
		});
		VisualElement visualElement2 = root.Q<VisualElement>("MusicVolume");
		_musicVolumeValueLabel = visualElement2.Q<Label>("Value");
		_musicVolumeSlider = InitializeSlider(visualElement2, _musicVolumeValueLabel, delegate(float v)
		{
			_soundSettings.MusicVolume = v;
		});
		VisualElement visualElement3 = root.Q<VisualElement>("EnvironmentVolume");
		_environmentVolumeValueLabel = visualElement3.Q<Label>("Value");
		_environmentVolumeSlider = InitializeSlider(visualElement3, _environmentVolumeValueLabel, delegate(float v)
		{
			_soundSettings.EnvironmentVolume = v;
		});
		VisualElement visualElement4 = root.Q<VisualElement>("UIVolume");
		_uiVolumeElementValueLabel = visualElement4.Q<Label>("Value");
		_uiVolumeElementSlider = InitializeSlider(visualElement4, _uiVolumeElementValueLabel, delegate(float v)
		{
			_soundSettings.UIVolume = v;
		});
		_muteWhenMinimizedToggle = root.Q<Toggle>("MuteWhenMinimized");
		_muteWhenMinimizedToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_soundSettings.MuteWhenMinimized = v.newValue;
		});
	}

	public void Update()
	{
		float valueWithoutNotify = Mathf.Clamp01(_soundSettings.MasterVolume);
		_masterVolumeSlider.SetValueWithoutNotify(valueWithoutNotify);
		_masterVolumeValueLabel.text = valueWithoutNotify.ToString("P0");
		float valueWithoutNotify2 = Mathf.Clamp01(_soundSettings.MusicVolume);
		_musicVolumeSlider.SetValueWithoutNotify(valueWithoutNotify2);
		_musicVolumeValueLabel.text = valueWithoutNotify2.ToString("P0");
		float valueWithoutNotify3 = Mathf.Clamp01(_soundSettings.EnvironmentVolume);
		_environmentVolumeSlider.SetValueWithoutNotify(valueWithoutNotify3);
		_environmentVolumeValueLabel.text = valueWithoutNotify3.ToString("P0");
		float valueWithoutNotify4 = Mathf.Clamp01(_soundSettings.UIVolume);
		_uiVolumeElementSlider.SetValueWithoutNotify(valueWithoutNotify4);
		_uiVolumeElementValueLabel.text = valueWithoutNotify4.ToString("P0");
		_muteWhenMinimizedToggle.SetValueWithoutNotify(_soundSettings.MuteWhenMinimized);
	}

	private static Slider InitializeSlider(VisualElement root, TextElement valueLabel, Action<float> setter)
	{
		Slider slider = root.Q<Slider>("Slider");
		slider.lowValue = MinVolume;
		slider.highValue = MaxVolume;
		slider.RegisterValueChangedCallback(delegate(ChangeEvent<float> v)
		{
			setter(ClampVolume(v.newValue));
			valueLabel.text = v.newValue.ToString("P0");
		});
		return slider;
	}

	private static float ClampVolume(float value)
	{
		return Mathf.Clamp(value, MinVolume, MaxVolume);
	}
}
