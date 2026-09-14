using System;
using JetBrains.Annotations;
using Timberborn.SettingsSystem;
using UnityEngine;

namespace Timberborn.CoreUI;

public class UISettings
{
	public static readonly float UIScaleStep = 0.01f;

	private static readonly string ShowFpsKey = "ShowFPS";

	private static readonly string UIScaleFactorKey = "UIScaleFactor";

	private static readonly string RunInBackgroundKey = "RunInBackground";

	private readonly ISettings _settings;

	public bool ShowFps
	{
		get
		{
			return _settings.GetBool(ShowFpsKey);
		}
		set
		{
			_settings.SetBool(ShowFpsKey, value);
		}
	}

	[UsedImplicitly]
	public bool HasStoredUIScaleFactor => _settings.Has(UIScaleFactorKey);

	public float UIScaleFactor
	{
		get
		{
			return (float)Mathf.RoundToInt(_settings.GetSafeFloat(UIScaleFactorKey, 1f) / UIScaleStep) * UIScaleStep;
		}
		set
		{
			_settings.SetFloat(UIScaleFactorKey, value);
			UIScaleFactorChanged?.Invoke(this, new SettingChangedEventArgs<float>(value));
		}
	}

	public bool RunInBackground
	{
		get
		{
			return _settings.GetBool(RunInBackgroundKey);
		}
		set
		{
			_settings.SetBool(RunInBackgroundKey, value);
			RunInBackgroundChanged?.Invoke(this, new SettingChangedEventArgs<bool>(value));
		}
	}

	public event EventHandler<SettingChangedEventArgs<float>> UIScaleFactorChanged;

	public event EventHandler<SettingChangedEventArgs<bool>> RunInBackgroundChanged;

	public UISettings(ISettings settings)
	{
		_settings = settings;
	}
}
