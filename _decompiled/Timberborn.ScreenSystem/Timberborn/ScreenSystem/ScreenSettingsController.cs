using System;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Timberborn.ScreenSystem;

internal class ScreenSettingsController : ILoadableSingleton
{
	private static readonly float ResolutionScaleTolerance = 0.01f;

	private readonly ScreenSettings _screenSettings;

	private readonly CommandLineScreenSettings _commandLineScreenSettings;

	private readonly ScreenSettingsLogger _screenSettingsLogger;

	public ScreenSettingsController(ScreenSettings screenSettings, CommandLineScreenSettings commandLineScreenSettings, ScreenSettingsLogger screenSettingsLogger)
	{
		_screenSettings = screenSettings;
		_commandLineScreenSettings = commandLineScreenSettings;
		_screenSettingsLogger = screenSettingsLogger;
	}

	public void Load()
	{
		SubscribeToSettingsEvents();
		UpdateSettings();
	}

	private void UpdateSettings()
	{
		UpdateScreenResolution();
		UpdateResolutionScale();
		UpdateVSyncCount();
		UpdateFrameRateLimit();
		_screenSettingsLogger.LogBrightnessChange();
	}

	private void SubscribeToSettingsEvents()
	{
		_screenSettings.ScreenResolutionChanged += delegate
		{
			UpdateSettings();
		};
		_screenSettings.FullScreenChanged += delegate
		{
			UpdateSettings();
		};
		_screenSettings.ResolutionScaleChanged += delegate
		{
			UpdateSettings();
		};
		_screenSettings.VSyncCountChanged += delegate
		{
			UpdateSettings();
		};
		_screenSettings.BrightnessChanged += delegate
		{
			UpdateSettings();
		};
		_screenSettings.FrameRateLimitChanged += delegate
		{
			UpdateSettings();
		};
	}

	private void UpdateScreenResolution()
	{
		ScreenResolution screenResolution = _screenSettings.ScreenResolution;
		FullScreenMode fullScreenMode = (_screenSettings.FullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
		ScreenResolution currentResolution = new ScreenResolution(Screen.width, Screen.height);
		if (fullScreenMode != Screen.fullScreenMode || screenResolution.Width != currentResolution.Width || screenResolution.Height != currentResolution.Height)
		{
			Screen.SetResolution(screenResolution.Width, screenResolution.Height, fullScreenMode);
			_screenSettingsLogger.LogResolutionChange(currentResolution, screenResolution);
		}
	}

	private void UpdateResolutionScale()
	{
		float resolutionScale = _screenSettings.ResolutionScale;
		if (Math.Abs(GetCurrentRenderPipeline().renderScale - resolutionScale) > ResolutionScaleTolerance)
		{
			GetCurrentRenderPipeline().renderScale = resolutionScale;
			_screenSettingsLogger.LogResolutionScaleChange();
		}
	}

	private void UpdateVSyncCount()
	{
		if (!Application.isEditor)
		{
			int num = ((!_commandLineScreenSettings.Uncapped) ? _screenSettings.VSyncCount : 0);
			if (QualitySettings.vSyncCount != num)
			{
				QualitySettings.vSyncCount = num;
				_screenSettingsLogger.LogVSyncCountChange();
			}
		}
	}

	private void UpdateFrameRateLimit()
	{
		if (!Application.isEditor)
		{
			int desiredTargetFrameRate = GetDesiredTargetFrameRate();
			if (Application.targetFrameRate != desiredTargetFrameRate)
			{
				Application.targetFrameRate = desiredTargetFrameRate;
				_screenSettingsLogger.LogFrameRateLimitChange();
			}
		}
	}

	private int GetDesiredTargetFrameRate()
	{
		if (_screenSettings.VSyncCount != 0 || !_screenSettings.FrameRateLimit.HasValue || _commandLineScreenSettings.Uncapped)
		{
			return -1;
		}
		return _screenSettings.FrameRateLimit.Value;
	}

	private static UniversalRenderPipelineAsset GetCurrentRenderPipeline()
	{
		return (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
	}
}
