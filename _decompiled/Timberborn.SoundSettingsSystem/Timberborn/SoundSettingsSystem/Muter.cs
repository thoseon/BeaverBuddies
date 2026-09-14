using System;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using UnityEngine;

namespace Timberborn.SoundSettingsSystem;

internal class Muter : ILoadableSingleton, IUnloadableSingleton
{
	private readonly ISoundSystem _soundSystem;

	private readonly SoundSettings _soundSettings;

	private readonly LoadingScreen _loadingScreen;

	public Muter(ISoundSystem soundSystem, SoundSettings soundSettings, LoadingScreen loadingScreen)
	{
		_soundSystem = soundSystem;
		_soundSettings = soundSettings;
		_loadingScreen = loadingScreen;
	}

	public void Load()
	{
		if (Application.isPlaying)
		{
			Application.focusChanged += OnFocusChanged;
			_loadingScreen.LoadingScreenEnabled += OnLoadingScreenEnabled;
			_loadingScreen.LoadingScreenDisabled += OnLoadingScreenDisabled;
		}
	}

	public void Unload()
	{
		Application.focusChanged -= OnFocusChanged;
		_loadingScreen.LoadingScreenEnabled -= OnLoadingScreenEnabled;
		_loadingScreen.LoadingScreenDisabled -= OnLoadingScreenDisabled;
	}

	private void OnFocusChanged(bool hasFocus)
	{
		if (hasFocus)
		{
			Unmute();
		}
		else if (_soundSettings.MuteWhenMinimized)
		{
			Mute();
		}
	}

	private void OnLoadingScreenEnabled(object sender, EventArgs e)
	{
		Mute();
	}

	private void OnLoadingScreenDisabled(object sender, EventArgs e)
	{
		UnmuteIfInFocus();
	}

	private void Mute()
	{
		_soundSystem.SetMasterVolume(0f);
	}

	private void UnmuteIfInFocus()
	{
		if (Application.isFocused || !_soundSettings.MuteWhenMinimized)
		{
			Unmute();
		}
	}

	private void Unmute()
	{
		_soundSystem.SetMasterVolume(_soundSettings.MasterVolume);
	}
}
