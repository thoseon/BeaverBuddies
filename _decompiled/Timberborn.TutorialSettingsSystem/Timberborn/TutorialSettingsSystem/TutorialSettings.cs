using System;
using Timberborn.SettingsSystem;

namespace Timberborn.TutorialSettingsSystem;

public class TutorialSettings
{
	private static readonly string DisableTutorialKey = "DisableTutorial2022-12-05";

	private readonly ISettings _settings;

	public bool DisableTutorial
	{
		get
		{
			return _settings.GetBool(DisableTutorialKey);
		}
		set
		{
			_settings.SetBool(DisableTutorialKey, value);
			DisableTutorialChanged?.Invoke(this, new SettingChangedEventArgs<bool>(value));
		}
	}

	public event EventHandler<SettingChangedEventArgs<bool>> DisableTutorialChanged;

	public TutorialSettings(ISettings settings)
	{
		_settings = settings;
	}
}
