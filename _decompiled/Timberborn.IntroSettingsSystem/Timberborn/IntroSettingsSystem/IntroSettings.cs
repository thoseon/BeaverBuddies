using Timberborn.SettingsSystem;

namespace Timberborn.IntroSettingsSystem;

public class IntroSettings
{
	private static readonly string DisableIntroKey = "DisableIntro";

	private readonly ISettings _settings;

	public bool DisableIntro
	{
		get
		{
			return _settings.GetBool(DisableIntroKey);
		}
		set
		{
			_settings.SetBool(DisableIntroKey, value);
		}
	}

	public IntroSettings(ISettings settings)
	{
		_settings = settings;
	}
}
