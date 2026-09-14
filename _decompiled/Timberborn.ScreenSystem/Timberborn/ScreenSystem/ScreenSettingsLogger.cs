using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ScreenSystem;

public class ScreenSettingsLogger : IPostLoadableSingleton
{
	private readonly ScreenSettings _screenSettings;

	private bool _initialLog;

	private bool _loggingEnabled;

	public ScreenSettingsLogger(ScreenSettings screenSettings)
	{
		_screenSettings = screenSettings;
	}

	public void PostLoad()
	{
		_initialLog = true;
		_loggingEnabled = true;
		ScreenResolution screenResolution = new ScreenResolution(Screen.width, Screen.height);
		LogResolutionChange(screenResolution, screenResolution);
		LogResolutionScaleChange();
		LogVSyncCountChange();
		LogBrightnessChange();
		LogFrameRateLimitChange();
		_initialLog = false;
	}

	public void LogResolutionChange(ScreenResolution currentResolution, ScreenResolution desiredResolution)
	{
		Display main = Display.main;
		Log($"Previous resolution: {currentResolution.Width} x {currentResolution.Height}" + $"\nNew resolution {desiredResolution.Width} x {desiredResolution.Height}" + $"\nDisplay resolution: {main.systemWidth} x {main.systemHeight}" + $"\nFull screen: {_screenSettings.FullScreen}");
	}

	public void LogResolutionScaleChange()
	{
		Log($"Resolution scale: {_screenSettings.ResolutionScale}");
	}

	public void LogVSyncCountChange()
	{
		Log($"VSync count: {_screenSettings.VSyncCount}");
	}

	public void LogBrightnessChange()
	{
		Log($"Brightness: {_screenSettings.Brightness}");
	}

	public void LogFrameRateLimitChange()
	{
		Log($"Frame rate limit: {_screenSettings.FrameRateLimit}");
	}

	private void Log(string logText)
	{
		if (_loggingEnabled && !Application.isEditor)
		{
			Debug.Log(_initialLog ? (logText ?? "") : ("Screen settings changed:\n" + logText));
		}
	}
}
