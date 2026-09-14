using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;

namespace Timberborn.SoundSettingsSystem;

internal class SoundSettingsUpdater : ILoadableSingleton
{
	private readonly ISoundSystem _soundSystem;

	private readonly SoundSettings _soundSettings;

	public SoundSettingsUpdater(ISoundSystem soundSystem, SoundSettings soundSettings)
	{
		_soundSystem = soundSystem;
		_soundSettings = soundSettings;
	}

	public void Load()
	{
		SetSoundSystemVolumes();
		SubscribeToSettingsEvents();
	}

	private void SetSoundSystemVolumes()
	{
		_soundSystem.SetMasterVolume(_soundSettings.MasterVolume);
		_soundSystem.SetMusicVolume(_soundSettings.MusicVolume);
		_soundSystem.SetEnvironmentVolume(_soundSettings.EnvironmentVolume);
		_soundSystem.SetUIVolume(_soundSettings.UIVolume);
	}

	private void SubscribeToSettingsEvents()
	{
		_soundSettings.MasterVolumeChanged += delegate(object _, SettingChangedEventArgs<float> e)
		{
			_soundSystem.SetMasterVolume(e.Value);
		};
		_soundSettings.MusicVolumeChanged += delegate(object _, SettingChangedEventArgs<float> e)
		{
			_soundSystem.SetMusicVolume(e.Value);
		};
		_soundSettings.EnvironmentVolumeChanged += delegate(object _, SettingChangedEventArgs<float> e)
		{
			_soundSystem.SetEnvironmentVolume(e.Value);
		};
		_soundSettings.UIVolumeChanged += delegate(object _, SettingChangedEventArgs<float> e)
		{
			_soundSystem.SetUIVolume(e.Value);
		};
	}
}
