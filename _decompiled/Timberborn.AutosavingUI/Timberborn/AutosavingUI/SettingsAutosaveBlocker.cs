using Timberborn.Autosaving;
using Timberborn.SettingsSystem;
using Timberborn.SettingsSystemUI;
using Timberborn.SingletonSystem;

namespace Timberborn.AutosavingUI;

internal class SettingsAutosaveBlocker : IAutosaveBlocker, ILoadableSingleton
{
	private readonly GameSavingSettings _gameSavingSettings;

	public bool IsBlocking { get; private set; }

	public SettingsAutosaveBlocker(GameSavingSettings gameSavingSettings)
	{
		_gameSavingSettings = gameSavingSettings;
	}

	public void Load()
	{
		IsBlocking = !_gameSavingSettings.AutoSavingOn;
		_gameSavingSettings.AutoSavingOnChanged += delegate(object _, SettingChangedEventArgs<bool> e)
		{
			IsBlocking = !e.Value;
		};
	}
}
