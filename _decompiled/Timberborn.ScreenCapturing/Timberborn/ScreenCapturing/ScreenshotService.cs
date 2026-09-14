using System;
using System.IO;
using Timberborn.FileSystem;
using Timberborn.InputSystem;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using Timberborn.UISound;
using UnityEngine;

namespace Timberborn.ScreenCapturing;

internal class ScreenshotService : IPriorityInputProcessor, ILoadableSingleton, ILateUpdatableSingleton
{
	private static readonly string ScreenshotKey = "Screenshot";

	private static readonly string ScreenshotUpscaledKey = "ScreenshotUpscaled";

	private static readonly string ScreenshotSoundName = "UI.Screenshot";

	private static readonly int UpscalingFactor = 2;

	private readonly InputService _inputService;

	private readonly IFileService _fileService;

	private readonly UISoundController _uiSoundController;

	private bool _shouldCaptureScreenshot;

	private bool _shouldCaptureUpscaledScreenshot;

	private static string ScreenshotsPath => Path.Combine(UserDataFolder.Folder, "Screenshots");

	public ScreenshotService(InputService inputService, IFileService fileService, UISoundController uiSoundController)
	{
		_inputService = inputService;
		_fileService = fileService;
		_uiSoundController = uiSoundController;
	}

	public void Load()
	{
		_inputService.AddInputProcessor(this);
	}

	public void ProcessInput()
	{
		_shouldCaptureScreenshot = _inputService.IsKeyDown(ScreenshotKey);
		_shouldCaptureUpscaledScreenshot = _inputService.IsKeyDown(ScreenshotUpscaledKey);
	}

	public void LateUpdateSingleton()
	{
		if (_shouldCaptureScreenshot)
		{
			CaptureScreenshot(upscale: false);
			_shouldCaptureScreenshot = false;
		}
		if (_shouldCaptureUpscaledScreenshot)
		{
			CaptureScreenshot(upscale: true);
			_shouldCaptureUpscaledScreenshot = false;
		}
	}

	private void CaptureScreenshot(bool upscale)
	{
		_fileService.CreateDirectory(ScreenshotsPath);
		int num = ((!upscale) ? 1 : UpscalingFactor);
		ScreenCapture.CaptureScreenshot(GetScreenshotFilePath(num), num);
		_uiSoundController.PlaySound(ScreenshotSoundName);
	}

	private string GetScreenshotFilePath(int upscalingFactor)
	{
		string text = $"{Screen.width * upscalingFactor}x{Screen.height * upscalingFactor}";
		string text2 = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH\\hmm\\mss\\s");
		string path = text + " - " + text2 + ".png";
		return _fileService.CombineIntoPath(ScreenshotsPath, path);
	}
}
