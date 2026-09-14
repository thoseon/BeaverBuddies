using System;
using Timberborn.SettingsSystem;

namespace Timberborn.SettingsSystemUI;

public class GameSavingSettings
{
	private static readonly string AutoSavingOnKey = "AutoSavingOn";

	private readonly ISettings _settings;

	public bool AutoSavingOn
	{
		get
		{
			return _settings.GetBool(AutoSavingOnKey, defaultValue: true);
		}
		set
		{
			_settings.SetBool(AutoSavingOnKey, value);
			AutoSavingOnChanged?.Invoke(this, new SettingChangedEventArgs<bool>(value));
		}
	}

	public event EventHandler<SettingChangedEventArgs<bool>> AutoSavingOnChanged;

	public GameSavingSettings(ISettings settings)
	{
		_settings = settings;
	}
}
