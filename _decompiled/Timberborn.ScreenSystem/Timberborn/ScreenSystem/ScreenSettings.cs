using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ScreenSystem;

public class ScreenSettings : ILoadableSingleton
{
	public static readonly ImmutableArray<int> VSyncValues = ImmutableArray.Create(0, 1, 2);

	public static readonly ImmutableArray<int?> FrameRateLimitValues = ((IEnumerable<int?>)new int?[12]
	{
		null, 30, 60, 80, 100, 120, 144, 160, 165, 180,
		200, 240
	}).ToImmutableArray();

	private static readonly int DefaultVSyncCount = 1;

	private static readonly int DefaultFrameRateLimit = 120;

	private static readonly string ResolutionWidthKey = "ResolutionWidth";

	private static readonly string ResolutionHeightKey = "ResolutionHeight";

	private static readonly string FullScreenKey = "FullScreen";

	private static readonly string ResolutionScaleKey = "ResolutionScale";

	private static readonly string VSyncCountKey = "VSyncCount";

	private static readonly string BrightnessKey = "Brightness";

	private static readonly string FrameRateLimitEnabledKey = "FrameRateLimitEnabled";

	private static readonly string FrameRateLimitKey = "FrameRateLimit";

	private readonly ISettings _settings;

	public ScreenResolution ScreenResolution
	{
		get
		{
			return new ScreenResolution(_settings.GetSafeInt(ResolutionWidthKey, Display.main.systemWidth), _settings.GetSafeInt(ResolutionHeightKey, Display.main.systemHeight));
		}
		set
		{
			_settings.SetInt(ResolutionWidthKey, value.Width);
			_settings.SetInt(ResolutionHeightKey, value.Height);
			ScreenResolutionChanged?.Invoke(this, new SettingChangedEventArgs<ScreenResolution>(value));
		}
	}

	public bool FullScreen
	{
		get
		{
			return _settings.GetSafeBool(FullScreenKey, defaultValue: true);
		}
		set
		{
			_settings.SetBool(FullScreenKey, value);
			FullScreenChanged?.Invoke(this, new SettingChangedEventArgs<bool>(value));
		}
	}

	public float ResolutionScale
	{
		get
		{
			return _settings.GetSafeFloat(ResolutionScaleKey, 1f);
		}
		set
		{
			_settings.SetFloat(ResolutionScaleKey, value);
			ResolutionScaleChanged?.Invoke(this, new SettingChangedEventArgs<float>(ResolutionScale));
		}
	}

	public int VSyncCount
	{
		get
		{
			return _settings.GetSafeInt(VSyncCountKey, 1);
		}
		set
		{
			_settings.SetInt(VSyncCountKey, value);
			VSyncCountChanged?.Invoke(this, new SettingChangedEventArgs<int>(value));
		}
	}

	public float Brightness
	{
		get
		{
			return _settings.GetSafeFloat(BrightnessKey, 1f);
		}
		set
		{
			_settings.SetFloat(BrightnessKey, value);
			BrightnessChanged?.Invoke(this, new SettingChangedEventArgs<float>(value));
		}
	}

	public int? FrameRateLimit
	{
		get
		{
			if (!_settings.GetSafeBool(FrameRateLimitEnabledKey))
			{
				return null;
			}
			return _settings.GetSafeInt(FrameRateLimitKey, DefaultFrameRateLimit);
		}
		set
		{
			_settings.SetBool(FrameRateLimitEnabledKey, value.HasValue);
			if (value.HasValue)
			{
				_settings.SetInt(FrameRateLimitKey, value.Value);
			}
			else
			{
				_settings.Clear(FrameRateLimitKey);
			}
			FrameRateLimitChanged?.Invoke(this, new SettingChangedEventArgs<int?>(value));
		}
	}

	public event EventHandler<SettingChangedEventArgs<ScreenResolution>> ScreenResolutionChanged;

	public event EventHandler<SettingChangedEventArgs<bool>> FullScreenChanged;

	public event EventHandler<SettingChangedEventArgs<float>> ResolutionScaleChanged;

	public event EventHandler<SettingChangedEventArgs<int>> VSyncCountChanged;

	public event EventHandler<SettingChangedEventArgs<float>> BrightnessChanged;

	public event EventHandler<SettingChangedEventArgs<int?>> FrameRateLimitChanged;

	public ScreenSettings(ISettings settings)
	{
		_settings = settings;
	}

	public void Load()
	{
		_settings.ValidateInt(VSyncCountKey, VSyncValues, DefaultVSyncCount);
		ImmutableArray<int> validValues = FrameRateLimitValues.Where((int? value) => value.HasValue).Cast<int>().ToImmutableArray();
		_settings.ValidateInt(FrameRateLimitKey, validValues, DefaultFrameRateLimit);
	}
}
