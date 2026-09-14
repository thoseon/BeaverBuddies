using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.GraphicsQualitySystem;

public class GraphicsQualitySettings : ILoadableSingleton
{
	private static readonly ImmutableArray<string> AllowedPresets = ((IEnumerable<string>)new string[5] { "Low", "Medium", "High", "Ultra", "Custom" }).ToImmutableArray();

	private static readonly GraphicsQualityPreset DefaultPreset = GraphicsQualityPreset.High;

	private static readonly string GraphicsQualityPresetKey = "GraphicsQualityPreset";

	private static readonly string AnisotropicFilteringKey = "AnisotropicTexturesQuality";

	private static readonly string AntiAliasingTypeKey = "AntiAliasingType";

	private static readonly string LightQualityKey = "LightQuality";

	private static readonly string ShadowQualityKey = "ShadowQuality";

	private static readonly string TextureQualityKey = "TextureQuality";

	private static readonly string WaterQualityKey = "WaterQuality";

	private static readonly string BloomKey = "Bloom";

	private readonly ISettings _settings;

	public string OverallGraphicsQuality
	{
		get
		{
			return _settings.GetSafeString(GraphicsQualityPresetKey, DefaultPreset.ToString());
		}
		set
		{
			_settings.SetString(GraphicsQualityPresetKey, value);
			GraphicsQualityPreset preset = GetPreset(value);
			if (preset != GraphicsQualityPreset.Custom)
			{
				AnisotropicFilteringEnabled = AnisotropicFilteringSetting.GetValueForPreset(preset);
				AntiAliasingType = AntiAliasingTypeSetting.GetValueForPreset(preset);
				LightQuality = LightQualitySetting.GetValueForPreset(preset);
				ShadowQuality = ShadowQualityGraphicsSettings.GetValueForPreset(preset);
				TextureQuality = TextureQualitySetting.GetValueForPreset(preset);
				WaterQuality = WaterQualitySetting.GetValueForPreset(preset);
				BloomEnabled = BloomSetting.GetValueForPreset(preset);
			}
		}
	}

	public bool AnisotropicFilteringEnabled
	{
		get
		{
			if (!IsStandardPreset(out var preset))
			{
				return _settings.GetSafeBool(AnisotropicFilteringKey, AnisotropicFilteringSetting.GetValueForPreset(DefaultPreset));
			}
			return AnisotropicFilteringSetting.GetValueForPreset(preset);
		}
		set
		{
			_settings.SetBool(AnisotropicFilteringKey, value);
			AnisotropicFilteringQualityChanged?.Invoke(this, new SettingChangedEventArgs<bool>(value));
		}
	}

	public AntialiasingType AntiAliasingType
	{
		get
		{
			return GetAntiAliasing();
		}
		set
		{
			_settings.SetInt(AntiAliasingTypeKey, (int)value);
			AntiAliasingTypeChanged?.Invoke(this, new SettingChangedEventArgs<AntialiasingType>(value));
		}
	}

	public int LightQuality
	{
		get
		{
			if (!IsStandardPreset(out var preset))
			{
				return _settings.GetSafeInt(LightQualityKey, LightQualitySetting.GetValueForPreset(DefaultPreset));
			}
			return LightQualitySetting.GetValueForPreset(preset);
		}
		set
		{
			_settings.SetInt(LightQualityKey, value);
			LightQualityChanged?.Invoke(this, new SettingChangedEventArgs<int>(value));
		}
	}

	public int ShadowQuality
	{
		get
		{
			if (!IsStandardPreset(out var preset))
			{
				return _settings.GetSafeInt(ShadowQualityKey, ShadowQualityGraphicsSettings.GetValueForPreset(DefaultPreset));
			}
			return ShadowQualityGraphicsSettings.GetValueForPreset(preset);
		}
		set
		{
			_settings.SetInt(ShadowQualityKey, value);
			ShadowQualityChanged?.Invoke(this, new SettingChangedEventArgs<int>(value));
		}
	}

	public int TextureQuality
	{
		get
		{
			if (!IsStandardPreset(out var preset))
			{
				return _settings.GetSafeInt(TextureQualityKey, TextureQualitySetting.GetValueForPreset(DefaultPreset));
			}
			return TextureQualitySetting.GetValueForPreset(preset);
		}
		set
		{
			_settings.SetInt(TextureQualityKey, value);
			TextureQualityChanged?.Invoke(this, new SettingChangedEventArgs<int>(value));
		}
	}

	public int WaterQuality
	{
		get
		{
			if (!IsStandardPreset(out var preset))
			{
				return _settings.GetSafeInt(WaterQualityKey, WaterQualitySetting.GetValueForPreset(DefaultPreset));
			}
			return WaterQualitySetting.GetValueForPreset(preset);
		}
		set
		{
			_settings.SetInt(WaterQualityKey, value);
			WaterQualityChanged?.Invoke(this, new SettingChangedEventArgs<int>(value));
		}
	}

	public bool BloomEnabled
	{
		get
		{
			if (!IsStandardPreset(out var preset))
			{
				return _settings.GetSafeBool(BloomKey, BloomSetting.GetValueForPreset(DefaultPreset));
			}
			return BloomSetting.GetValueForPreset(preset);
		}
		set
		{
			_settings.SetBool(BloomKey, value);
			BloomChanged?.Invoke(this, new SettingChangedEventArgs<bool>(value));
		}
	}

	public event EventHandler<SettingChangedEventArgs<bool>> AnisotropicFilteringQualityChanged;

	public event EventHandler<SettingChangedEventArgs<AntialiasingType>> AntiAliasingTypeChanged;

	public event EventHandler<SettingChangedEventArgs<int>> LightQualityChanged;

	public event EventHandler<SettingChangedEventArgs<int>> ShadowQualityChanged;

	public event EventHandler<SettingChangedEventArgs<int>> TextureQualityChanged;

	public event EventHandler<SettingChangedEventArgs<int>> WaterQualityChanged;

	public event EventHandler<SettingChangedEventArgs<bool>> BloomChanged;

	private GraphicsQualitySettings(ISettings settings)
	{
		_settings = settings;
	}

	public void ChangeToCustom()
	{
		OverallGraphicsQuality = "Custom";
	}

	public void Load()
	{
		EnsureBackwardPresetCompatibility();
		ValidateSavedSettings();
	}

	private void EnsureBackwardPresetCompatibility()
	{
		string safeString = _settings.GetSafeString("GraphicsQuality", string.Empty);
		string safeString2 = _settings.GetSafeString(GraphicsQualityPresetKey, string.Empty);
		if (safeString != string.Empty && safeString2 == string.Empty)
		{
			OverallGraphicsQuality = safeString;
		}
	}

	private void ValidateSavedSettings()
	{
		_settings.ValidateString(GraphicsQualityPresetKey, AllowedPresets, "High");
		_settings.ValidateInt(AntiAliasingTypeKey, AntiAliasingTypeSetting.ValidValues, (int)AntiAliasingTypeSetting.GetValueForPreset(DefaultPreset));
		_settings.ValidateInt(LightQualityKey, LightQualitySetting.ValidValues, LightQualitySetting.GetValueForPreset(DefaultPreset));
		_settings.ValidateInt(ShadowQualityKey, ShadowQualityGraphicsSettings.ValidValues, ShadowQualityGraphicsSettings.GetValueForPreset(DefaultPreset));
		_settings.ValidateInt(TextureQualityKey, TextureQualitySetting.ValidValues, TextureQualitySetting.GetValueForPreset(DefaultPreset));
		_settings.ValidateInt(WaterQualityKey, WaterQualitySetting.ValidValues, WaterQualitySetting.GetValueForPreset(DefaultPreset));
	}

	private bool IsStandardPreset(out GraphicsQualityPreset preset)
	{
		preset = GetPreset(OverallGraphicsQuality);
		return preset != GraphicsQualityPreset.Custom;
	}

	private static GraphicsQualityPreset GetPreset(string presetName)
	{
		return presetName switch
		{
			"Low" => GraphicsQualityPreset.Low, 
			"Medium" => GraphicsQualityPreset.Medium, 
			"High" => GraphicsQualityPreset.High, 
			"Ultra" => GraphicsQualityPreset.Ultra, 
			_ => GraphicsQualityPreset.Custom, 
		};
	}

	private AntialiasingType GetAntiAliasing()
	{
		if (IsStandardPreset(out var preset))
		{
			return AntiAliasingTypeSetting.GetValueForPreset(preset);
		}
		int safeInt = _settings.GetSafeInt(AntiAliasingTypeKey, -1);
		if (safeInt >= 0)
		{
			return (AntialiasingType)safeInt;
		}
		string key = "AntiAliasingQuality";
		if (_settings.GetSafeInt(key, -1) <= 1)
		{
			return AntialiasingType.Off;
		}
		return AntiAliasingTypeSetting.GetValueForPreset(DefaultPreset);
	}
}
