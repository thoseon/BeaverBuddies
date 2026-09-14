using System;
using Timberborn.SettingsSystem;

namespace Timberborn.SoundSettingsSystem;

public class SoundSettings
{
	private static readonly string MasterVolumeKey = "MasterVolume";

	private static readonly string MusicVolumeKey = "MusicVolume";

	private static readonly string EnvironmentVolumeKey = "EnvironmentVolume";

	private static readonly string UIVolumeKey = "UIVolume";

	private static readonly string MuteWhenMinimizedKey = "MuteWhenMinimized";

	private readonly ISettings _settings;

	public float MasterVolume
	{
		get
		{
			return _settings.GetFloat(MasterVolumeKey, 1f);
		}
		set
		{
			_settings.SetFloat(MasterVolumeKey, value);
			MasterVolumeChanged?.Invoke(this, new SettingChangedEventArgs<float>(value));
		}
	}

	public float MusicVolume
	{
		get
		{
			return _settings.GetFloat(MusicVolumeKey, 1f);
		}
		set
		{
			_settings.SetFloat(MusicVolumeKey, value);
			MusicVolumeChanged?.Invoke(this, new SettingChangedEventArgs<float>(value));
		}
	}

	public float EnvironmentVolume
	{
		get
		{
			return _settings.GetFloat(EnvironmentVolumeKey, 1f);
		}
		set
		{
			_settings.SetFloat(EnvironmentVolumeKey, value);
			EnvironmentVolumeChanged?.Invoke(this, new SettingChangedEventArgs<float>(value));
		}
	}

	public float UIVolume
	{
		get
		{
			return _settings.GetFloat(UIVolumeKey, 1f);
		}
		set
		{
			_settings.SetFloat(UIVolumeKey, value);
			UIVolumeChanged?.Invoke(this, new SettingChangedEventArgs<float>(value));
		}
	}

	public bool MuteWhenMinimized
	{
		get
		{
			return _settings.GetBool(MuteWhenMinimizedKey, defaultValue: true);
		}
		set
		{
			_settings.SetBool(MuteWhenMinimizedKey, value);
		}
	}

	public event EventHandler<SettingChangedEventArgs<float>> MasterVolumeChanged;

	public event EventHandler<SettingChangedEventArgs<float>> MusicVolumeChanged;

	public event EventHandler<SettingChangedEventArgs<float>> EnvironmentVolumeChanged;

	public event EventHandler<SettingChangedEventArgs<float>> UIVolumeChanged;

	public SoundSettings(ISettings settings)
	{
		_settings = settings;
	}
}
